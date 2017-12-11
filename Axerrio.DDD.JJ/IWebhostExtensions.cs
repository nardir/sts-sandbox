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
                    
                    //Todo: extensionmethod on context: migrateEnumerations                    
                    var enumerationProperties = context.GetType().GetProperties()
                        .Where(p => p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>))
                        .Where(p => typeof(IEnumeration).IsAssignableFrom(p.PropertyType.GetGenericArguments().First()));
                                        
                    foreach(var enumerationProperty in enumerationProperties)
                    {
                        var dbSetGenericPropertyType = enumerationProperty.PropertyType.GetGenericArguments().First();

                        //Creating instance of type without default constructor in C# using reflection: https://stackoverflow.com/questions/390578/creating-instance-of-type-without-default-constructor-in-c-sharp-using-reflectio
                        dynamic enumeration = FormatterServices.GetUninitializedObject(dbSetGenericPropertyType);
                        var items = (IEnumerable)enumeration.Items;

                        //https://stackoverflow.com/questions/33940507/find-a-generic-dbset-in-a-dbcontext-dynamically
                        var model = context.GetType()
                                .GetRuntimeProperties()
                                .Where(o =>
                                    o.PropertyType.IsGenericType &&
                                    o.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>) &&
                                    o.PropertyType.GenericTypeArguments.Contains(dbSetGenericPropertyType))
                                .FirstOrDefault();

                        dynamic dbSet = model.GetValue(context);

                        foreach (dynamic item in items)
                        {
                            var id = item.Id;
                            var name = item.Name;                            
                            //Opmerking: name .ToLowerInvariant() -->  Waarom?
                                                        
                            var existingItem = dbSet.Find(id);
                            if(existingItem == null)
                                dbSet.Add(item);
                            else
                            {
                                //Opmerking: public string Name { get; private set; } --> private weggehaald hiervoor.
                                existingItem.Name = name;
                                dbSet.Update(existingItem);
                            }
                        }

                        context.SaveChanges();
                    }

                    seeder(context, services);

                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"An error occurred while migrating the database used on context {typeof(TContext).Name}");
                }
                finally
                {

                    logger.LogInformation($"Migrated database associated with context {typeof(TContext).Name}");
                }

            }

            return webHost;
        }        

        public static IWebHost MigrateDbContext<TContext>(this IWebHost webHost) where TContext : DbContext
        {
            return webHost.MigrateDbContext<TContext>((_, __) => { });
        }
    }
}
