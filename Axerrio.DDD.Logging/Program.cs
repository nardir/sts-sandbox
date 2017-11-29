using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.Channel;

namespace Axerrio.DDD.Logging
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //https://github.com/serilog/serilog/wiki/Configuration-Basics
            //https://github.com/serilog/serilog-aspnetcore/tree/dev/samples/SimpleWebSample

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
                .Build();

            var InstrumentationKey = configuration.GetValue<string>("ApplicationInsights:InstrumentationKey");

            //https://github.com/serilog/serilog-sinks-applicationinsights
            //var telemetryConfiguration = new TelemetryConfiguration()
            //{
            //    InstrumentationKey = InstrumentationKey,
            //    TelemetryChannel = new PersistenceChannel()
            //};

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                //.WriteTo.Debug()
                //.WriteTo.ApplicationInsightsTraces(InstrumentationKey)
                //.WriteTo.ApplicationInsightsTraces(telemetryConfiguration)
                .CreateLogger();

            try
            {
                Log.Information("Getting the motors running...");

                var host = WebHost.CreateDefaultBuilder(args)
                    .UseApplicationInsights(InstrumentationKey)
                    .UseConfiguration(configuration)
                    .UseSerilog()
                    .UseStartup<Startup>()
                    .Build();

                host.Run();
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
    }
}
