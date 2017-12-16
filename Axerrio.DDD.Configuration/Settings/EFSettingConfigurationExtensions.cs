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
        public static IConfigurationBuilder AddEntityFrameworkSettings<TContext>(this IConfigurationBuilder builder, Action<DbContextOptionsBuilder<TContext>> optionsAction) 
            where TContext : DbContext, ISettingDbContext, new()
        {
            EnsureArg.IsNotNull(optionsAction, nameof(optionsAction));

            builder.Add(new EFSettingConfigurationSource<TContext>(optionsAction));

            return builder;
        }

        public static IConfigurationBuilder AddEntityFrameworkSettings<TContext>(this IConfigurationBuilder builder, TContext context)
            where TContext : DbContext, ISettingDbContext, new()
        {
            EnsureArg.IsNotNull(context, nameof(context));

            builder.Add(new EFSettingConfigurationSource<TContext>(context));

            return builder;
        }
    }
}
