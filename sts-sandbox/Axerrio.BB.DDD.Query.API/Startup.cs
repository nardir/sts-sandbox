using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Axerrio.BB.DDD.Infrastructure.Query.ModelBinder;
using Axerrio.BB.DDD.Query.API.Data;
using Axerrio.BB.DDD.Query.API.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Axerrio.BB.DDD.Query.API
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
            var connectionString = @"Server=MAUI\HALEAKALA;Database=WideWorldImporters;Trusted_Connection=True;";

            services.AddEntityFrameworkSqlServer()
                .AddDbContextPool<WorldWideImportersQueryContext>(options =>
                {
                    options.UseSqlServer(connectionString, sqlOptions =>
                    {
                        sqlOptions.MigrationsAssembly(typeof(WorldWideImportersQueryContext).GetTypeInfo().Assembly.GetName().Name);
                    });
                });

            services.AddTransient<IQueryService, QueryService>();

            services.AddMvc()
                .AddQueryOptions();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
