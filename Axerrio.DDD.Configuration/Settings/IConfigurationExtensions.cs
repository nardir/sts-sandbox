using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.DDD.Configuration.Settings
{
    public static class IConfigurationExtensions
    {
        public static T Get<T>(this IConfiguration configuration, string key, T defaultValue) where T : class
        {
            T value = null;

            IConfiguration section = configuration.GetSection(key);
            
            if (section != null)
                value = section.Get<T>();

            if (value == null)
                value = defaultValue;

            return value;
        }

        public static T Get<T>(this IConfiguration configuration, string key, Func<T> defaultValueFunc) where T : class
        {
            return configuration.Get<T>(key, defaultValueFunc());
        }
    }
}
