using accounts.Data;
using accounts.Models;
using id_sts.Extensions;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
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
                    var userManager = services.GetService<UserManager<ApplicationUser>>();

                    ApplicationUserSeed.SeedAsync(userManager).Wait();
                })
                .Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
    }
}