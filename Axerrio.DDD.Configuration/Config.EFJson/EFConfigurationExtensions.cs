using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.DDD.Configuration.Config.EFJson
{
    public static class EFConfigurationExtensions
    {
        public static IConfigurationBuilder AddEntityFrameworkSettings(this IConfigurationBuilder configurationBuilder)
        {
            return configurationBuilder;
        }
    }
}
