using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Axerrio.DDD.Configuration.Settings;
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
                    var config = builder.Build();
                    
                    //var data = config.AsEnumerable();

                    var connectionString = config["ConnectionString"];

                    //var options = new DbContextOptionsBuilder<SettingDbContext>()
                    //    .UseInMemoryDatabase("Settings Config")
                    //    .Options;

                    //var ctx = new SettingDbContext(options);

                    //var testOptions = new TestOptions()
                    //{
                    //    Id = 100,
                    //    Description = "NR Test Options",
                    //    Names = new string[] { "aap", "noot", "mies" }
                    //};

                    //var logger = new LoggerFactory().CreateLogger<EFSettingService>();

                    //var settingService = new EFSettingService(ctx, logger);

                    //var setting = settingService.UpdateOrAddAsync(nameof(TestOptions), testOptions).Result;

                    Func<ISettingService, ILogger, Task> seeder = (service, logger) => 
                    {
                        var testOptions = new TestOptions()
                        {
                            Id = 100,
                            Description = "NR Test Options",
                            Names = new string[] { "aap", "noot", "mies" }
                        };

                        return service.AddIfNotExistsAsync(nameof(TestOptions), testOptions);
                    };

                    builder.AddEntityFrameworkSettings<SettingDbContext, EFSettingService>(options =>
                    {
                        //options.UseInMemoryDatabase("Settings Config");
                        options.UseSqlServer(connectionString);
                    }
                    , new LoggerFactory()
                    , seeder);

                })
                .UseStartup<Startup>()
                .Build();
    }
}
