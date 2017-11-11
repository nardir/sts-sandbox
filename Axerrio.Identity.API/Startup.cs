using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Axerrio.Identity.Accounts.Data;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Axerrio.Identity.Accounts.Models;
using Microsoft.AspNetCore.Identity;
using Axerrio.Identity.API.Configuration;
using Axerrio.Identity.API.Services;
using IdentityServer4.Services;

namespace Axerrio.Identity.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            var connectionString = Configuration["ConnectionString"];

            services.AddDbContext<ApplicationDbContext>(options =>
             options.UseSqlServer(connectionString,
                                     sqlServerOptionsAction: sqlOptions =>
                                     {
                                         sqlOptions.MigrationsAssembly(typeof(ApplicationDbContext).GetTypeInfo().Assembly.GetName().Name);
                                     }));

            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
                    {
                        options.Password.RequireDigit = false;
                        options.Password.RequiredLength = 4;
                        options.Password.RequireNonAlphanumeric = false;
                        options.Password.RequireUppercase = false;

                        options.User.RequireUniqueEmail = true;
                    })
                    .AddEntityFrameworkStores<ApplicationDbContext>()
                    .AddDefaultTokenProviders();

            services.AddTransient<IRedirectService, RedirectService>();
            services.AddTransient<ILoginService, LoginService>();
            //services.AddTransient<IMessageService, FileMessageService>(); //SendGridMessageService
            services.AddTransient<IMessageService, SendGridMessageService>();

            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;


            //PersistedGrantDbContext
            services.AddIdentityServer(options => options.IssuerUri = "null") //options => options.IssuerUri = "null"
                    //.AddDeveloperSigningCredential()
                    .AddSigningCredential(Certificate.Certificate.Get())
                    //.AddInMemoryIdentityResources(Config.GetIdentityResources())
                    //.AddInMemoryApiResources(Config.GetApiResources())
                    //.AddInMemoryClients(Config.GetClients())
                    .AddAspNetIdentity<ApplicationUser>()
                    .AddConfigurationStore(options =>
                    {
                        options.ConfigureDbContext = builder => builder.UseSqlServer(connectionString,
                                        sqlServerOptionsAction: sqlOptions =>
                                        {
                                            sqlOptions.MigrationsAssembly(migrationsAssembly);
                                        });
                    })
                    .AddOperationalStore(options =>
                    {
                        options.ConfigureDbContext = builder => builder.UseSqlServer(connectionString,
                                        sqlServerOptionsAction: sqlOptions =>
                                        {
                                            sqlOptions.MigrationsAssembly(migrationsAssembly);
                                        });
                        //options.DefaultSchema = "ops";
                    })
                    .Services.AddTransient<IProfileService, ProfileService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            //app.UseAuthentication();

            app.UseIdentityServer(); //http://localhost:5000/.well-known/openid-configuration       

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
