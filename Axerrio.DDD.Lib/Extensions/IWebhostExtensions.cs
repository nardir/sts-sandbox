using Axerrio.BuildingBlocks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Axerrio.BuildingBlocks
{
    public static class IWebhostExtensions
    {
        public static IWebHost MigrateDbContext<TContext>(this IWebHost webHost, Action<TContext, IServiceProvider> seeder) where TContext : DbContext
        {
            using (var scope = webHost.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                var logger = services.GetRequiredService<ILogger<TContext>>();

                var context = services.GetService<TContext>();

                try
                {
                    logger.LogInformation($"Migrating database associated with context {typeof(TContext).Name}");

                    context.Database.Migrate();
                    
                    //Opmerking: scope meegeven of logger meegeven
                    context.MigrateEnumerations(logger);

                    seeder(context, services);

                }
                catch (Exception ex)
                {
                    //Opmerking: MicroService start nu gewoon op. Mag dat?
                    logger.LogError(ex, $"An error occurred while migrating the database used on context {typeof(TContext).Name}");
                }
                finally
                {

                    logger.LogInformation($"Migrated database associated with context {typeof(TContext).Name}");
                }

            }

            return webHost;
        }

        //Todo: extensionmethod zit in verkeerde class nu.
        private static void MigrateEnumerations<TContext>(this TContext context, ILogger<TContext> logger) where TContext : DbContext
        {
            logger.LogInformation($"Migrating database enumeration sets associated with context {typeof(TContext).Name}");
                          
            var enumerationProperties = context.GetType().GetProperties()
                .Where(p => p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>))
                .Where(p => typeof(IEnumeration).IsAssignableFrom(p.PropertyType.GetGenericArguments().First()));

            foreach (var enumerationProperty in enumerationProperties)
            {
                var dbSetGenericPropertyType = enumerationProperty.PropertyType.GetGenericArguments().First();

                //Creating instance of type without default constructor in C# using reflection: https://stackoverflow.com/questions/390578/creating-instance-of-type-without-default-constructor-in-c-sharp-using-reflectio
                dynamic enumeration = FormatterServices.GetUninitializedObject(dbSetGenericPropertyType);
                var enumerationItems = (IEnumerable)enumeration.Items;

                //Get instance of specific DbSet: https://stackoverflow.com/questions/33940507/find-a-generic-dbset-in-a-dbcontext-dynamically
                var model = context.GetType()
                        .GetRuntimeProperties()
                        .Where(o =>
                            o.PropertyType.IsGenericType &&
                            o.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>) &&
                            o.PropertyType.GenericTypeArguments.Contains(dbSetGenericPropertyType))
                        .FirstOrDefault();

                dynamic dbSet = model.GetValue(context);

                List<int> existingIds = new List<int>();
                foreach (dynamic item in enumerationItems)
                {
                    int id = item.Id;
                    var name = item.Name;
                    //Opmerking: name .ToLowerInvariant() -->  Waarom?

                    var existingItem = dbSet.Find(id);
                    if (existingItem == null)
                    {
                        dbSet.Add(item);
                        logger.LogInformation($"Added enumeration value for type {dbSetGenericPropertyType.Name}. Id:{item.Id}, Name:'{item.Name}'");
                    }
                    else if (existingItem.Name != name)
                    {
                        var existingName = existingItem.Name;
                        //Opmerking: public string Name { get; private set; } --> private weggehaald hiervoor.
                        existingItem.Name = name;
                        dbSet.Update(existingItem);

                        logger.LogInformation($"Updated enumeration value for type {dbSetGenericPropertyType.Name}. Id:{item.Id}, Name:'{existingName}' updated to '{item.Name}'");
                    }

                    existingIds.Add(id);
                }

                //ForEach & if needed, due too: Cannot use a lambda expression as an argument to a dynamically dispatched operation
                foreach (dynamic item in dbSet)
                {
                    if (!existingIds.Contains(item.Id))
                    {
                        dbSet.Remove(item);

                        logger.LogInformation($"Deleted enumeration value for type {dbSetGenericPropertyType.Name}. Id:{item.Id}, Name:'{item.Name}'");
                    }
                }

                context.SaveChanges();
            }

            logger.LogInformation($"Migrated database enumeration sets associated with context {typeof(TContext).Name}");
        }

        public static IWebHost MigrateDbContext<TContext>(this IWebHost webHost) where TContext : DbContext
        {
            return webHost.MigrateDbContext<TContext>((_, __) => { });
        }
    }
}
