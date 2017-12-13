﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Axerrio.DDD.Configuration.Infrastructure;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Axerrio.DDD.Configuration
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, builder) => 
                {
                    //var options = new DbContextOptionsBuilder<ConfigurationContext>()
                    //    .UseInMemoryDatabase("Config")
                    //    .Options;

                    var options = new TestOptions()
                    {
                        Id = 100,
                        Description = "NR Test Options",
                        Names = new string[] { "aap", "noot", "mies" }
                    };

                    //var input = JsonConvert.SerializeObject(options);
                    //var data = JsonConfigurationParser.Parse(nameof(TestOptions), input);

                    var data = JsonConfigurationParser.Parse(nameof(TestOptions), options);

                    builder.AddInMemoryCollection(data);
                })
                .UseStartup<Startup>()
                .Build();
    }
}
