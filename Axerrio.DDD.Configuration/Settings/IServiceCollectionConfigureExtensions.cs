using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.DDD.Configuration.Settings
{
    public static class IServiceCollectionConfigureExtensions
    {
        public static IServiceCollection Configure<TOptions>(this IServiceCollection services, IConfiguration configuration, string key, Action<TOptions> configureDefaultOptions = null) where TOptions : class
        {
            //Name variant
            if (configuration.SectionExists(key))
                return services.Configure<TOptions>(configuration.GetSection(key));

            return services.Configure(configureDefaultOptions);
        }
    }
}
