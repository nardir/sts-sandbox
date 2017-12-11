﻿using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

using Axerrio.IWebHostsExtensions;
using Axerrio.BuildingBlocks;
using Axerrio.DDD.Menu.Infrastructure;
using Serilog;
using Microsoft.Extensions.Configuration;
using System.IO;
using System;

namespace Axerrio.DDD.Messaging
{
    public class Program
    {
        //TODO:
        //- Multiple projects for this test case.
        //- Axerrio.DDD.Lib structure/folders 
        //- What to test?

        //Ideas:
        //- Create Default Axerrio API/Microservices Project template(s) based on Building Blocks.
        //- Create Class templates based on Building Blcoks (DomainEvent etc.)


        public static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
               .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
               .Build();

            //Setup Serilog
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .Enrich.FromLogContext()
                .WriteTo.Console()
 //               .WriteTo.Debug()
                .CreateLogger();

            try
            {
                Log.Information("Starting web host");

                BuildWebHost(args)
                .MigrateDbContext<ClientRequestContext>()
                .MigrateDbContext<MenuContext>()
                .Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseSerilog()
                .Build();
    }
}
