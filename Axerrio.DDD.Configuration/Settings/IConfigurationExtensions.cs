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
        }
    }
}
