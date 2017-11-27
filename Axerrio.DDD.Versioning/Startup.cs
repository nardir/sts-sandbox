using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc;
using Axerrio.DDD.Versioning.Controllers;
using Microsoft.AspNetCore.Mvc.Versioning;

namespace Axerrio.DDD.Versioning
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

            //https://tpodolak.com/blog/2017/05/04/asp-net-core-api-versioning-convention/

            //https://tpodolak.com/blog/2017/05/03/asp-net-core-default-api-version-with-url-path-versioning/

            //https://dotnetcoretutorials.com/2017/01/17/api-versioning-asp-net-core/

            //https://www.codeproject.com/Articles/1204149/Versioning-ASP-NET-Core-Web-API

            //https://github.com/Microsoft/aspnet-api-versioning/wiki/New-Services-Quick-Start

            services.AddApiVersioning(o =>
            {
                o.ReportApiVersions = true;
                o.AssumeDefaultVersionWhenUnspecified = true;
                o.ApiVersionReader = new HeaderApiVersionReader("x-api-version");
                o.DefaultApiVersion = new ApiVersion(3, 0);
                
                o.Conventions.Controller<ValuesController>().HasApiVersion(new ApiVersion(2, 0));
                o.Conventions.Controller<ValuesController>().HasApiVersion(new ApiVersion(3, 0));

                o.Conventions.Controller<ValuesV1Controller>().HasApiVersion(new ApiVersion(1, 0));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.UseMvc();

            //app.UseMvc(routes =>
            //{
            //    //routes.MapRoute(
            //    //    name: "default",
            //    //    template: "{controller=Home}/{action=Index}/{id?}");

            //    routes.MapRoute(
            //        name: "v1",
            //        template: "api/v1/{controller}/{action}");

            //    routes.MapRoute(
            //        name: "v2",
            //        template: "api/v2/{controller}/{action}");

            //    routes.MapRoute(
            //        name: "v3",
            //        template: "api/v3/{controller}/{action}");

            //});
        }
    }
}
