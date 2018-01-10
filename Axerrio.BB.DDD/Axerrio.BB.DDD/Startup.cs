﻿using Axerrio.BB.DDD.Application.IntegrationEvents;
using Axerrio.BB.DDD.Application.IntegrationEvents.Abstractions;
using Axerrio.BB.DDD.EntityFrameworkCore.Infrastructure.IntegrationEvents;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Axerrio.BB.DDD
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
            var connectionString = Configuration.GetValue<string>("ConnectionString");

            services.AddDbContext<OrderingDbContext>(options => 
            {
                options.UseSqlServer(connectionString, sqlOptions => 
                {
                    sqlOptions.MigrationsAssembly(typeof(OrderingDbContext).GetTypeInfo().Assembly.GetName().Name);
                });
            });

            services.AddTransient<IIntegrationEventsEnqueueService, IntegrationEventsEnqueueService<OrderingDbContext>>();
            services.AddSingleton<IIntegrationEventsQueueService, EFCoreIntegrationEventsQueueService<OrderingDbContext>>();
            services.AddTransient<IEventBusStoreAndForward, StoreAndForwardEventBus>();
            services.AddTransient<IIntegrationEventsService, StoreAndForwardIntegrationEventsService>();
            services.AddSingleton<IEventBus, RabbitMQEventBus>();
            services.AddSingleton<IEventBusPublishOnly>(provider => 
            {
                return provider.GetRequiredService<IEventBus>();
            });
            

            //var pm = new PaymentMethod(1, "VISA", "1234-4567-9999-1111", "123", "Piet", DateTime.UtcNow.AddYears(1));
            //var pmjson = JsonConvert.SerializeObject(pm);
            //var pmrestored = JsonConvert.DeserializeObject<PaymentMethod>(pmjson);

            //var ie = new PaymentMethodCreatedIntegrationEvent(pm);
            //var iejson = JsonConvert.SerializeObject(ie);
            //var ierestored = JsonConvert.DeserializeObject<PaymentMethodCreatedIntegrationEvent>(iejson);

            services.AddMvc();
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
