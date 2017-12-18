﻿using EnsureThat;
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

            var optionsbuilder = new DbContextOptionsBuilder<TContext>();
            optionsAction(optionsbuilder);
            using (var context = (TContext)Activator.CreateInstance(typeof(TContext), optionsbuilder.Options))
            {
                Migrate(context);
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

            Migrate(context);

            builder.Add(new EFSettingConfigurationSource<TContext, TSettingService>(context, loggerFactory, seeder));

            return builder;
        }

        private static void Migrate<TContext>(TContext context)
            where TContext : DbContext, ISettingDbContext
        {
            if (context.Database.IsSqlServer())
            {
                context.Database.Migrate();
            }
        }
    }
}
