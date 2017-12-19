using EnsureThat;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.DDD.Configuration.Settings
{
    public static class EFSettingConfigurationExtensions
    { 
        public static IConfigurationBuilder AddEntityFrameworkSettings<TContext, TSettingService>(this IConfigurationBuilder builder
                , Action<DbContextOptionsBuilder<TContext>> optionsAction
                , ILoggerFactory loggerFactory
                , Func<ISettingService, ILogger, Task> seeder = null) 
            where TContext : DbContext, ISettingDbContext, new()
            where TSettingService : ISettingService
        {
            EnsureArg.IsNotNull(optionsAction, nameof(optionsAction));
            EnsureArg.IsNotNull(loggerFactory, nameof(loggerFactory));

            var optionsbuilder = new DbContextOptionsBuilder<TContext>();
            optionsAction(optionsbuilder);
            using (var context = (TContext)Activator.CreateInstance(typeof(TContext), optionsbuilder.Options))
            {
                Migrate(context, loggerFactory.CreateLogger(nameof(EFSettingConfigurationExtensions)));
                Seed<TContext, TSettingService>(context, seeder, loggerFactory);
            }

            builder.Add(new EFSettingConfigurationSource<TContext>(optionsAction, loggerFactory));

            return builder;
        }

        public static IConfigurationBuilder AddEntityFrameworkSettings<TContext, TSettingService>(this IConfigurationBuilder builder
                , TContext context
                , ILoggerFactory loggerFactory
                , Func<ISettingService, ILogger, Task> seeder = null)
            where TContext : DbContext, ISettingDbContext, new()
            where TSettingService : ISettingService
        {
            EnsureArg.IsNotNull(context, nameof(context));
            EnsureArg.IsNotNull(loggerFactory, nameof(loggerFactory));

            Migrate(context, loggerFactory.CreateLogger(nameof(EFSettingConfigurationExtensions)));
            Seed<TContext, TSettingService>(context, seeder, loggerFactory);

            builder.Add(new EFSettingConfigurationSource<TContext>(context, loggerFactory));

            return builder;
        }

        private static void Seed<TContext, TSettingService>(TContext context, Func<ISettingService, ILogger, Task> seeder, ILoggerFactory loggerFactory)
            where TContext : DbContext, ISettingDbContext, new()
            where TSettingService : ISettingService
        {
            var logger = loggerFactory.CreateLogger(nameof(EFSettingConfigurationExtensions));

            if (seeder != null)
            {
                var connectionStringBuilder = new SqlConnectionStringBuilder(context.Database.GetDbConnection().ConnectionString);

                logger.LogDebug($"Seeding settings for database {connectionStringBuilder.DataSource} {connectionStringBuilder.InitialCatalog}");

                var service = (TSettingService)Activator.CreateInstance(typeof(TSettingService), context, loggerFactory.CreateLogger<TSettingService>());

                seeder(service, logger).Wait();

                logger.LogDebug($"Seeded settings for database {connectionStringBuilder.DataSource} {connectionStringBuilder.InitialCatalog}");
            }
            else
            {
                logger.LogDebug($"No settings to seed, no setting seeder was supplied");
            }
        }

        private static void Migrate<TContext>(TContext context, ILogger logger)
            where TContext : DbContext, ISettingDbContext
        {
            if (context.Database.IsSqlServer())
            {
                var connectionStringBuilder = new SqlConnectionStringBuilder(context.Database.GetDbConnection().ConnectionString);

                logger.LogDebug($"Migrating setting database objects for database {connectionStringBuilder.DataSource} {connectionStringBuilder.InitialCatalog}");

                context.Database.Migrate();

                logger.LogDebug($"Migrated setting database objects for database {connectionStringBuilder.DataSource} {connectionStringBuilder.InitialCatalog}");
            }
        }
    }
}
