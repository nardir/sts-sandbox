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
        public static IServiceCollection Configure<TOptions>(this IServiceCollection services, IConfiguration configuration, Action<TOptions> configureDefaultOptions = null, string key = null) where TOptions : class, new()
        {
            //var option = Activator.CreateInstance<TOptions>();
            var option = new TOptions();
            var optionType = option.GetType();

            string sectionName = key ?? optionType.Name;

            var section = configuration.GetSection(sectionName);

            if (section.Value != null)
                return services.Configure<TOptions>(section);

            return services.Configure(configureDefaultOptions);
        }
    }
}
