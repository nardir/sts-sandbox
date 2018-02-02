using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Axerrio.BB.DDD.IE.API.Application;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Axerrio.BB.DDD.IE.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //BuildWebHost(args).Run();

            var webHost = BuildWebHost(args);

            //var provider = webHost.Services;
            //using (var scope = provider.CreateScope())
            //{
            //    var context = scope.ServiceProvider.GetRequiredService<OrderingDbContext>();

            //    context.Database.Migrate();
            //}

            webHost.Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
    }
}
