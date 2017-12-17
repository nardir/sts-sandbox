using EnsureThat;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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

            builder.Add(new EFSettingConfigurationSource<TContext, TSettingService>(optionsAction, seeder));

            return builder;
        }

        public static IConfigurationBuilder AddEntityFrameworkSettings<TContext, TSettingService>(this IConfigurationBuilder builder
                , TContext context
                , Func<ISettingService, Task> seeder = null)
            where TContext : DbContext, ISettingDbContext, new()
            where TSettingService : ISettingService
        {
            EnsureArg.IsNotNull(context, nameof(context));

            Migrate(context);

            builder.Add(new EFSettingConfigurationSource<TContext, TSettingService>(context, seeder));

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
