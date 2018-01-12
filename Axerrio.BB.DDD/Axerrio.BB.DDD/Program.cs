using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Axerrio.BB.DDD
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var webHost = BuildWebHost(args);

            //var provider = webHost.Services;
            //using (var scope = provider.CreateScope())
            //{
            //    var context = scope.ServiceProvider.GetRequiredService<OrderingDbContext>();

            //    context.Database.Migrate();
            //}

            
            
            webHost.Run();

            //Console.ReadLine();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseShutdownTimeout(TimeSpan.FromSeconds(10)) //Aanpassen shutdown tijd
                .UseStartup<Startup>()
                .Build();
    }
}
