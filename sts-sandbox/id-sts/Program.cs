using accounts.Data;
using id_sts.Extensions;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System;

namespace id_sts
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args)
                .MigrateDbContext<ApplicationDbContext>((context, services) =>
                {
                    //var env = services.GetService<IHostingEnvironment>();
                    //var logger = services.GetService<ILogger<ApplicationDbContextSeed>>();
                    //var settings = services.GetService<IOptions<AppSettings>>();

                    //new ApplicationDbContextSeed()
                    //    .SeedAsync(context, env, logger, settings)
                    //    .Wait();
                })
                .Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
    }
}

//.MigrateDbContext<ApplicationDbContext>((context, services) =>
//{
//    //var env = services.GetService<IHostingEnvironment>();
//    //var logger = services.GetService<ILogger<ApplicationDbContextSeed>>();
//    //var settings = services.GetService<IOptions<AppSettings>>();

//    //new ApplicationDbContextSeed()
//    //    .SeedAsync(context, env, logger, settings)
//    //    .Wait();
//})