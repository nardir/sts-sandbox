using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Axerrio.IWebHostExtensions;
using Axerrio.Identity.Accounts.Data;
using Axerrio.Identity.Accounts.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using Axerrio.Identity.API.Certificate;
using IdentityServer4.EntityFramework.DbContexts;
using Axerrio.Identity.API.Data;

namespace Axerrio.Identity.API
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
                .MigrateDbContext<PersistedGrantDbContext>()
                .MigrateDbContext<ConfigurationDbContext>((context, services) => 
                {
                    ConfigurationDbContextSeed.SeedAsync(context).Wait();
                })
                .Run();
        }

        //stssandboxkv : B8Ow71m+dCL1APjMHjwFypdCuHr1B0pd6ULQhzEu8f0=
        //sts-sandbox-kv : qfdPnh5tuR7DMgVM9jrDGVaYbAip0ognZDGy4Zw711M=

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args) //https://andrewlock.net/exploring-program-and-startup-in-asp-net-core-2-preview1-2/
                .UseKestrel(options =>
                {
                    options.Listen(IPAddress.Parse("127.0.0.1"), 5000);

                    //options.Listen(IPAddress.Parse("127.0.0.1"), 50000, listenOptions =>
                    //{
                    //    listenOptions.UseHttps(Certificate.Certificate.Get());
                    //});

                    using (var store = new X509Store(StoreName.My))
                    {
                        store.Open(OpenFlags.ReadOnly);
                        var certs = store.Certificates.Find(X509FindType.FindBySubjectName, "localhost", false);
                        if (certs.Count > 0)
                        {
                            var certificate = certs[0];

                            // listen for HTTPS
                            options.Listen(IPAddress.Parse("127.0.0.1"), 50000, listenOptions =>
                            {
                                listenOptions.UseHttps(certificate);
                            });
                        }
                    }
                })
                .ConfigureAppConfiguration((context, config) =>
                {
                    #region snippet1
                    config.SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("azurekeyvault.json", optional: false)
                        .AddEnvironmentVariables();

                    var builtConfig = config.Build();

                    config.AddAzureKeyVault(
                        $"https://{builtConfig["Vault"]}.vault.azure.net/",
                        builtConfig["ClientId"],
                        builtConfig["ClientSecret"]);
                    #endregion
                })
                .UseStartup<Startup>()
                .Build();

        //SendGridApiKey
    }
}
