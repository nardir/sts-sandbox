using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Axerrio.BuildingBlocks
{
    public static class DbContextExtensions
    {
        private static dynamic GetSetWithItemsInChangeTracker<TContext>(this TContext context, Type dbSetGenericType)
            where TContext : DbContext
        {
            //Get instance of the specific generic DbSet: https://stackoverflow.com/questions/33940507/find-a-generic-dbset-in-a-dbcontext-dynamically
            var model = context.GetType()
                                    .GetRuntimeProperties()
                                    .Where(o =>
                                        o.PropertyType.IsGenericType &&
                                        o.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>) &&
                                        o.PropertyType.GenericTypeArguments.Contains(dbSetGenericType))
                                    .FirstOrDefault();

            dynamic dbSet = model.GetValue(context);

            //Get all items of the dbSet in the change tracker with one db SELECT action, so we have all enumerations in memory.
            foreach (dynamic record in dbSet) { break; }

            return dbSet;
        }

        public static void MigrateEnumerations<TContext>(this TContext context, ILogger<TContext> logger) where TContext : DbContext
        {
            logger.LogInformation($"Migrating database enumeration sets associated with context {typeof(TContext).Name}");

            var enumerationDbSets = context.GetType().GetProperties()
                .Where(p => p.PropertyType.IsGenericType && typeof(DbSet<>) == p.PropertyType.GetGenericTypeDefinition())   //All DbSets with an Generic type
                .Where(p => typeof(IEnumeration).IsAssignableFrom(p.PropertyType.GetGenericArguments().First()));           //Only DbSets with generic Argument of Ienumeration

            foreach (var enumerationDbSet in enumerationDbSets)
            {
                var dbSetGenericType = enumerationDbSet.PropertyType.GetGenericArguments().First();     //Get the Type of the generic argument: DbSet has only one generic argument                                                                                                       

                //Make sure static constructor of the generic type is called. (and only once)! https://stackoverflow.com/questions/2524906/how-do-i-invoke-a-static-constructor-with-reflection/28738241#28738241
                RuntimeHelpers.RunClassConstructor(dbSetGenericType.TypeHandle);

                var enumerationItems = (IEnumerable)dbSetGenericType.BaseType.GetProperty("Items", BindingFlags.Static | BindingFlags.Public) //Get the static, public Property Items
                        .GetValue(null);   //GetValue null: static class, so no instantiated object to get the value from. Is ignored at static class.

                var dbSet = context.GetSetWithItemsInChangeTracker(dbSetGenericType);

                List<int> existingIds = new List<int>();
                foreach (dynamic item in enumerationItems)
                {
                    var existingItem = dbSet.Find(item.Id);
                    if (existingItem == null)
                    {
                        dbSet.Add(item);
                        logger.LogInformation($"Added enumeration value for type {dbSetGenericType.Name}. Id:{item.Id}, Name:'{item.Name}'");
                    }
                    else if (existingItem.Name != item.Name)
                    {
                        context.Entry(existingItem).CurrentValues.SetValues(item);
                        dbSet.Update(existingItem);
                        logger.LogInformation($"Updated enumeration value for type {dbSetGenericType.Name}. Id:{item.Id}, Name:'{existingItem.Name}' updated to '{item.Name}'");
                    }

                    existingIds.Add(item.Id);
                }

                //ForEach & if needed, due too: Cannot use a lambda expression as an argument to a dynamically dispatched operation
                foreach (dynamic item in dbSet)
                {
                    if (!existingIds.Contains(item.Id))
                    {
                        dbSet.Remove(item);

                        logger.LogInformation($"Deleted enumeration value for type {dbSetGenericType.Name}. Id:{item.Id}, Name:'{item.Name}'");
                    }
                }

                context.SaveChanges();
            }

            logger.LogInformation($"Migrated database enumeration sets associated with context {typeof(TContext).Name}");
        }
    }
}
