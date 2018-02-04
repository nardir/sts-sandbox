using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Axerrio.CQRS.API.Application.Query;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OData.Edm;

namespace Axerrio.CQRS.API
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
            //https://www.connectionstrings.com/sql-server-2016/
            var connectionString = @"Server=MAUI\HALEAKALA;Database=WideWorldImporters;Trusted_Connection=True;";

            services.AddTransient<ISalesQueries, SalesQueries>(provider => 
            {
                return new SalesQueries(connectionString);
            });

            //Moon
            //services.AddMvc()
            //    .AddOData(isCaseSensitive:false);

            services.AddMvc();

            services.AddOData();
        }

        private ODataConventionModelBuilder BuildEdmModel(IServiceProvider services)
        {
            //http://odata.github.io/WebApi/#14-01-netcore-beta1
            //https://github.com/OData/WebApi/tree/feature/netcore/src
            //https://github.com/OData/WebApi/issues/1179
            //http://www.odata.org/getting-started/basic-tutorial/#filter
            //https://www.codeproject.com/Articles/1227943/case-insensitive-filter-in-odata-aspnetcore-webapi

            // you can add all the entities you need
            var builder = new ODataConventionModelBuilder(services);

            
            builder.EntitySet<SalesOrder>("SalesOrders");
            builder.EntityType<SalesOrder>().HasKey(so => so.OrderId); // the call to HasKey is mandatory

            return builder;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //https://github.com/OData/WebApi/issues/1179

            var edmBuilder = BuildEdmModel(app.ApplicationServices);

            //app.UseMvc();
            app.UseMvc(routeBuilder => 
            {
                routeBuilder.Count().Filter().OrderBy().Expand().Select().MaxTop(null);

                //https://github.com/OData/WebApi/issues/812
                routeBuilder.MapODataServiceRoute("ODataRoute", "odata", edmBuilder.GetEdmModel());

                // Work-around for #1175
                routeBuilder.EnableDependencyInjection();
            });
        }
    }
}
