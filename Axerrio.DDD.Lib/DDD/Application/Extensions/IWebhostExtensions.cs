using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace Axerrio.BuildingBlocks
{
    //Axerrio.IWebhostExtensions --> heet nuget nu in axerrio nuget.
    //Axerrio.BuildingBlocks --> algemene namespaces alle nugets? 
    //      --> in algemene of aparte ... using Microsoft.AspNetCore.Hosting & EntityFrameworkCore nodig.

    //DDD --> of weglaten, is neit altijd DDD?
    //splitsen: 
    //Axerrio.BuildingBlocks.DDD.Application (
    // --> Hierin onderstaande. Domain heeft Microsoft.AspNetCore.Hosting niet nodig. IWebHost is api/application

    //Axerrio.BuildingBlocks.DDD.Domain

    //Axerrio.BuildingBlocks.DDD.Infrastructure

    public static class IWebhostExtensions
    {
        public static IWebHost MigrateDbContext<TContext>(this IWebHost webHost, Action<TContext, IServiceProvider> seeder, bool migrateEnumerations = false) where TContext : DbContext
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

        public static IWebHost MigrateDbContext<TContext>(this IWebHost webHost, bool migrateEnumerations) where TContext : DbContext
        {
            return webHost.MigrateDbContext<TContext>((_, __) => { }, migrateEnumerations);
        }

        public static IWebHost MigrateDbContext<TContext>(this IWebHost webHost) where TContext : DbContext
        {
            return webHost.MigrateDbContext<TContext>((_, __) => { });
        }
    }
}
