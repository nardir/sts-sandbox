using EnsureThat;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.DDD.Configuration.Settings
{
    public static class IServiceCollectionConfigureExtensions
    {
        public static IServiceCollection Configure<TOptions>(this IServiceCollection services, IConfiguration configuration, string name, string key, Action<TOptions> configureDefaultOptions) where TOptions : class
        {
            EnsureArg.IsNotNull(name, nameof(name));
            EnsureArg.IsNotNull(key, nameof(key));
            EnsureArg.IsNotNull(configureDefaultOptions, nameof(configureDefaultOptions));

            if (configuration.SectionExists(key))
                return services.Configure<TOptions>(name, configuration.GetSection(key));

            return services.Configure(name, configureDefaultOptions);
        }

        public static IServiceCollection Configure<TOptions>(this IServiceCollection services, IConfiguration configuration, string key, Action<TOptions> configureDefaultOptions) where TOptions : class
        {
            return services.Configure(configuration, Options.DefaultName, key, configureDefaultOptions);
        }

        public static IServiceCollection Configure<TOptions>(this IServiceCollection services, IConfiguration configuration, string name, string key) where TOptions : class
        {
            EnsureArg.IsNotNull(name, nameof(name));
            EnsureArg.IsNotNull(key, nameof(key));

            return services.Configure<TOptions>(name, configuration.GetSection(key));
        }

        public static IServiceCollection Configure<TOptions>(this IServiceCollection services, IConfiguration configuration, string key) where TOptions : class
        {
            return services.Configure<TOptions>(configuration, Options.DefaultName, key);
        }

        public static IServiceCollection Configure<TOptions>(this IServiceCollection services, string name, string key, Action<TOptions> configureOptions) where TOptions : class
        {
            EnsureArg.IsNotNull(name, nameof(name));
            EnsureArg.IsNotNull(key, nameof(key));
            EnsureArg.IsNotNull(configureOptions, nameof(configureOptions));

            return services.Configure(name: name, configureOptions: configureOptions);
        }

        public static IServiceCollection Configure<TOptions>(this IServiceCollection services, string key, Action<TOptions> configureOptions) where TOptions : class
        {
            return services.Configure(Options.DefaultName, key, configureOptions);
        }
    }
}
