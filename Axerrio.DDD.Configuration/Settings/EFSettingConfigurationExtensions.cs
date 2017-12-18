using EnsureThat;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.DDD.Configuration.Settings
{
    public static class EFSettingConfigurationExtensions
    { 
        public static IConfigurationBuilder AddEntityFrameworkSettings<TContext, TSettingService>(this IConfigurationBuilder builder
                , Action<DbContextOptionsBuilder<TContext>> optionsAction
                , ILoggerFactory loggerFactory
                , Func<ISettingService, Task> seeder = null) 
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
            }

            builder.Add(new EFSettingConfigurationSource<TContext, TSettingService>(optionsAction, loggerFactory, seeder));

            return builder;
        }

        public static IConfigurationBuilder AddEntityFrameworkSettings<TContext, TSettingService>(this IConfigurationBuilder builder
                , TContext context
                , ILoggerFactory loggerFactory
                , Func<ISettingService, Task> seeder = null)
            where TContext : DbContext, ISettingDbContext, new()
            where TSettingService : ISettingService
        {
            EnsureArg.IsNotNull(context, nameof(context));
            EnsureArg.IsNotNull(loggerFactory, nameof(loggerFactory));

            Migrate(context, loggerFactory.CreateLogger(nameof(EFSettingConfigurationExtensions)));

            builder.Add(new EFSettingConfigurationSource<TContext, TSettingService>(context, loggerFactory, seeder));

            return builder;
        }

        private static void Migrate<TContext>(TContext context, ILogger logger)
            where TContext : DbContext, ISettingDbContext
        {
            if (context.Database.IsSqlServer())
            {
                logger.LogDebug($"");

                context.Database.Migrate();
            }
        }
    }
}
