using System;
using System.Reflection;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using Axerrio.BuildingBlocks;
using MediatR;
using Axerrio.DDD.Menu.Infrastructure;
using Axerrio.DDD.Menu.Domain.AggregatesModel.MenuAggregate;
using Axerrio.DDD.Menu.Infrastructure.Repositories;
using Axerrio.DDD.Menu.Domain.AggregatesModel.ArtistAggregate;

namespace Axerrio.DDD.Menu
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

            services.AddDbContext<MenuContext>(options =>
            {
                options.UseSqlServer(Configuration["ConnectionString"],
                    sqlServerOptionsAction: sqlOptions =>
                    {
                        sqlOptions.MigrationsAssembly(typeof(MenuContext).GetTypeInfo().Assembly.GetName().Name);
                        sqlOptions.EnableRetryOnFailure(maxRetryCount: 10, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                    });
            });

            services.AddDbContext<ClientRequestContext>(options =>
            {
                options.UseSqlServer(Configuration["ConnectionString"],
                    sqlServerOptionsAction: sqlOptions =>
                    {
                        sqlOptions.MigrationsAssembly(typeof(ClientRequestContext).GetTypeInfo().Assembly.GetName().Name);                        
                        sqlOptions.EnableRetryOnFailure(maxRetryCount: 10, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                    });
            });

            //Todo: to seperate builders
            services.AddScoped<IMenuRepository, MenuRepository>();
            services.AddScoped<IArtistRepository, ArtistRepository>();

            services.AddTransient<IClientRequestService, ClientRequestService>();


            //IMediator zaken.
            services.AddMediatR(typeof(Startup).GetTypeInfo().Assembly);
 //           services.AddMediatR(typeof(MenuContext).GetTypeInfo().Assembly);

            services.AddSwaggerGen(options =>
            {
  //              options.DescribeAllEnumsAsStrings();
                options.SwaggerDoc("v1", new Swashbuckle.AspNetCore.Swagger.Info
                {
                    Title = "Menu HTTP API",
                    Version = "v1",
                    Description = "The Menu Service HTTP API",
                    TermsOfService = "Terms Of Service"
                });
            });


            //https://docs.microsoft.com/en-us/aspnet/core/tutorials/web-api-help-pages-using-swagger?tabs=visual-studio



        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger()
               .UseSwaggerUI(c =>
               {
                   c.SwaggerEndpoint($"/swagger/v1/swagger.json", "My Menu API V1");
                  
               });

            app.UseMvc();
        }
    }
}
