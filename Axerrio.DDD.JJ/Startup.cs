﻿using System;
using System.Reflection;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using MediatR;
using Axerrio.DDD.Menu.Infrastructure;
using Axerrio.DDD.Menu.Domain.AggregatesModel.MenuAggregate;
using Axerrio.DDD.Menu.Infrastructure.Repositories;
using Axerrio.DDD.Menu.Domain.AggregatesModel.ArtistAggregate;
using Axerrio.DDD.Menu.Application.Validations;
using Axerrio.BB.DDD.EntityFrameworkCore;
using Axerrio.BB.DDD.Infrastructure.Idempotency;
using Axerrio.BB.DDD.EntityFrameworkCore.Infrastructure.Idempotency;
using Axerrio.BB.DDD.FluentValidation.Application.Behaviors;
using Axerrio.BB.DDD.Application.Behaviors;
using Axerrio.BB.DDD.FluentValidation.Application.SchemaFilters;
using Axerrio.BB.DDD.EntityFrameworkCore.Infrastructure;
using Axerrio.DDD.Menu.Application.Queries;
using Microsoft.Extensions.Options;
using Axerrio.BB.AspNetCore.Extensions.Configuration;

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
            services.AddMvc()
                    .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<AddArtistCommandValidator>())
                    .AddOData();


            services.Configure<MenuDomainOptions>(Configuration);
            services.AddSingleton<IConfigureOptions<MenuDomainOptions>, ConfigureMenuDomainOptions>();

            //Todo: die hele settingstructuur!
            var queryOptions = new ReadQueryOptions() { ConnectionString = Configuration["ConnectionString"] };
            services.AddSingleton<ReadQueryOptions>(queryOptions);
           

            //Todo: ConnectionString options? MenuDomainOptions is teveel voor de MenuQueries?
            services.AddTransient<IReadQueries, DapperReadQueries>();


            //Todo: verwerkt BB hierin!
            services.AddDbContext<MenuContext>(options =>
            {
                options.UseSqlServer(Configuration["ConnectionString"],
                    sqlServerOptionsAction: sqlOptions =>
                    {
                        sqlOptions.MigrationsAssembly(typeof(MenuContext).GetTypeInfo().Assembly.GetName().Name);
                        sqlOptions.EnableRetryOnFailure(maxRetryCount: 10, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                    });
            });

            services.AddDbContext<ClientRequestDbContext>(options =>
            {
                options.UseSqlServer(Configuration["ConnectionString"],
                    sqlServerOptionsAction: sqlOptions =>
                    {
                        sqlOptions.MigrationsAssembly(typeof(ClientRequestDbContext).GetTypeInfo().Assembly.GetName().Name);                        
                        sqlOptions.EnableRetryOnFailure(maxRetryCount: 10, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                    });
            });

            //Todo: to seperate builders
            services.AddScoped<IMenuRepository, MenuRepository>();
            services.AddScoped<IArtistRepository, ArtistRepository>();

            services.AddTransient<IClientRequestService, ClientRequestService>();
                        
           

            //IMediator zaken.
            services.AddMediatR(typeof(Startup).GetTypeInfo().Assembly);
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidatorBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            
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

                //TODO: ODataOptions breken hierop!!
                //options.SchemaFilter<AddFluentValidationRules>(); 
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


