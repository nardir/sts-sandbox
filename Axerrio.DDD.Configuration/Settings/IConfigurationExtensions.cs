using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.DDD.Configuration.Settings
{
    public static class IConfigurationExtensions
    {
        public static bool SectionExists(this IConfiguration configuration, string key)
        {
            return configuration.GetChildren().Any(section => section.Key == key);
        }

        public static T Get<T>(this IConfiguration configuration, string key) where T : class
        {
            return configuration.Get(key, default(T));
        }

        public static T Get<T>(this IConfiguration configuration, string key, T defaultValue) where T : class
        {
            return configuration.SectionExists(key) ? configuration.GetSection(key).Get<T>() : defaultValue;

            //T value = configuration.TryGet<T>(key);

            ////IConfiguration section = configuration.GetSection(key);

            ////if (section != null)
            ////    value = section.Get<T>();

            //if (value == null)
            //    value = defaultValue;

            //return value;
        }

        public static T Get<T>(this IConfiguration configuration, string key, Func<T> defaultValueFunc) where T : class
        {
            return configuration.Get(key, defaultValueFunc());
        }
    }
}
