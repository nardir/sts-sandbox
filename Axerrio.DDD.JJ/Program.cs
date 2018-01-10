using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Axerrio.DDD.Menu.Infrastructure;
using Serilog;
using Microsoft.Extensions.Configuration;
using System.IO;
using System;
using Axerrio.BB.DDD.EntityFrameworkCore;
using Axerrio.BB.AspNetCore.EntityFrameworkCore.Extensions;
using Axerrio.BB.DDD.Domain;
using Axerrio.BB.DDD.EntityFrameworkCore.Infrastructure;
using Axerrio.BB.AspNetCore.EntityFrameworkCore.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Axerrio.BB.DDD.Infrastructure.Idempotency;
using EnsureThat;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Axerrio.DDD.Menu
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
                .MigrateDbContext<ClientRequestDbContext>()
                .MigrateDbContext<MenuContext>(typeof(IEnumeration))
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
