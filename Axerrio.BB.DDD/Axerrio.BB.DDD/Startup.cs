using Axerrio.BB.DDD.Application.IntegrationEvents;
using Axerrio.BB.DDD.Application.IntegrationEvents.Abstractions;
using Axerrio.BB.DDD.EntityFrameworkCore.Infrastructure.IntegrationEvents;
using Axerrio.BB.DDD.Infrastructure.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Quartz.Spi;
using System;
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

            services.AddOptions();

            services.Configure<IntegrationEventsDatabaseOptions>(options => 
            {
                options.Schema = "integrationevents";
                options.TableName = "EventQueueItem";
            });

            services.Configure<IntegrationEventsQueueServiceOptions>(options => 
            {
                options.MaxEventsToDequeue = 6;
            });

            services.AddDbContext<OrderingDbContext>(options => 
            {
                options.UseSqlServer(connectionString, sqlOptions => 
                {
                    sqlOptions.MigrationsAssembly(typeof(OrderingDbContext).GetTypeInfo().Assembly.GetName().Name);
                });
            });


            //TODO NR: Extensions methods to Add services
            services.AddTransient<IIntegrationEventsQueueService, EFCoreIntegrationEventsQueueService<OrderingDbContext>>();
            services.AddTransient<StoreAndForwardEventBus>();
            services.AddTransient<IIntegrationEventsService, StoreAndForwardIntegrationEventsService<StoreAndForwardEventBus>>();
            services.AddTransient<IIntegrationEventsForwarderService, IntegrationEventsForwarderService<RabbitMQEventBus>>();

            services.AddSingleton<RabbitMQEventBus>();
            services.AddSingleton<IEventBus>(provider => 
            {
                return provider.GetRequiredService<RabbitMQEventBus>();
            });
            
            services.AddTransient<IEventBusPublishOnlyFactory, EventBusPublishOnlyFactory>();

            
            services.AddSingleton<IJobFactory, JobFactory>();
            services.AddTransient<TestJob>();
            services.AddSingleton<TestTriggerFactory>();
            //services.AddSingleton<IHostedService, TimedHostedService<TestJob, TestTriggerFactory>>();
            //services.AddSingleton<IHostedService, TestHostedService>();

            //var pm = new PaymentMethod(1, "VISA", "1234-4567-9999-1111", "123", "Piet", DateTime.UtcNow.AddYears(1));
            //var pmjson = JsonConvert.SerializeObject(pm);
            //var pmrestored = JsonConvert.DeserializeObject<PaymentMethod>(pmjson);

            //var ie = new PaymentMethodCreatedIntegrationEvent(pm);
            //var iejson = JsonConvert.SerializeObject(ie);
            //var ierestored = JsonConvert.DeserializeObject<PaymentMethodCreatedIntegrationEvent>(iejson);

            //var queueItem = new IntegrationEventsQueueItem(ierestored);
            //var iefromQueueItem = queueItem.IntegrationEvent;

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
