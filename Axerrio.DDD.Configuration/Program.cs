using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
//using Axerrio.DDD.Configuration.Config.EFJson;
using Axerrio.DDD.Configuration.Infrastructure;
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

                    //var ctxoptions = new DbContextOptionsBuilder<JsonEFConfigurationContext>()
                    //    .UseSqlServer("")
                    //    .Options;

                    //using (var ctx = new JsonEFConfigurationContext(ctxoptions))
                    //{
                    //    if (ctx.Database.IsSqlServer())
                    //        ctx.Database.Migrate();
                    //}



                    //var options = new DbContextOptionsBuilder<ConfigurationContext>()
                    //    .UseInMemoryDatabase("Config")
                    //    .Options;

                    //var options = new TestOptions()
                    //{
                    //    Id = 100,
                    //    Description = "NR Test Options",
                    //    Names = new string[] { "aap", "noot", "mies" }
                    //};

                    //var input = JsonConvert.SerializeObject(options);
                    //var data = JsonConfigurationParser.Parse(nameof(TestOptions), input);

                    //var data = JsonConfigurationParser.Parse(nameof(TestOptions), options);

                    //builder.AddInMemoryCollection(data);

                    builder.AddEntityFrameworkSettings<SettingDbContext>(options => 
                    {
                        options.UseInMemoryDatabase("Settings Config");
                    });
                })
                .UseStartup<Startup>()
                .Build();
    }
}
