using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Axerrio.BB.DDD.EntityFrameworkCore.Infrastructure;
using Axerrio.BB.DDD.EntityFrameworkCore.Infrastructure.AutofacModules;
using Axerrio.BB.DDD.IE.API.Application;
using Axerrio.BB.DDD.Infrastructure.IntegrationEvents;
using Axerrio.BB.DDD.Infrastructure.IntegrationEvents.Abstractions;
using Axerrio.BB.AspNetCore.Extensions.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Axerrio.BB.DDD.IE.RabbitMQ.Infrastructure;
using Axerrio.BB.DDD.Infrastructure.AutofacModules;
using Axerrio.BB.DDD.Infrastructure.Options;
using Axerrio.BB.DDD.Dapper.Infrastructure.AutofacModules;

namespace Axerrio.BB.DDD.IE.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public IContainer ApplicationContainer { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            var connectionString = Configuration.GetValue<string>("ConnectionString");

            services.AddOptions();

            services.Configure<StoreAndForwardEventBusDatabaseOptions>(configuration: Configuration, key: "StoreAndForwardEventBusDatabaseOptions");
            services.Configure<StoreAndForwardEventBusDatabaseOptions>(options => 
            {
                options.ConnectionString = connectionString;
                options.MaxEventsToDequeue = 5;
                options.RetryAttempts = 3;
            });

            services.Configure<StoreAndForwardEventBusConsumerOptions>(configuration: Configuration, key: "StoreAndForwardEventBusForwarderOptions");
            services.Configure<StoreAndForwardEventBusConsumerOptions>(options => 
            {
                options.TriggerIntervalInMilliseconds = 10000;
                options.TriggerRequeingCronExpression = "0 0/30 * * * ?";
            });

            services.Configure<StoreAndForwardEventBusForwardOptions>(configuration: Configuration, key: "StoreAndForwardEventBusForwardOptions");
            services.Configure<StoreAndForwardEventBusForwardOptions>(options => 
            {
                options.MaxPublishAttempts = 3;
                options.RequeuePendingEventsPeriodInMinutes = 15;
            });

            services.Configure<EventBusOptions>(configuration: Configuration, key: "RabbitMQEventBus");

            services.AddEntityFrameworkSqlServer()
                .AddDbContext<IntegrationEventsDbContext>(options =>
                {
                    options.UseSqlServer(connectionString, sqlOptions =>
                    {
                        sqlOptions.MigrationsAssembly(typeof(IntegrationEventsDbContext).GetTypeInfo().Assembly.GetName().Name);
                        sqlOptions.EnableRetryOnFailure(maxRetryCount: 10, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                    });
                });
            

            //services.AddTransient<EFCoreStoreAndForwardEventBus<OrderingDbContext>>();

            services.AddMvc();

            //http://autofaccn.readthedocs.io/en/latest/integration/aspnetcore.html
            var builder = new ContainerBuilder();

            builder.Populate(services);

            //builder.RegisterModule(new EFCoreIntegrationEventsModule<OrderingDbContext>());

            builder.RegisterModule(new IntegrationEventHandlerModule<Startup>());
            builder.RegisterModule(new StoreAndForwardEventBusPublisherModule<IntegrationEventsDbContext>());
            builder.RegisterModule(new StoreAndForwardEventBusForwarderModule());
            builder.RegisterModule(new StoreAndForwardEventBusConsumerModule());
            builder.RegisterModule(new EventBusModule<RabbitMQEventBus>());
            
            ApplicationContainer = builder.Build();

            return new AutofacServiceProvider(ApplicationContainer);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime appLifetime)
        {
            appLifetime.ApplicationStopped.Register(() => ApplicationContainer.Dispose());

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //var subscriptionsService = app.ApplicationServices.GetRequiredService<IEventBusSubscriptionsService>();

            //subscriptionsService.AddSubscription<OrderCreatedIntegrationEvent, OrderCreatedIntegrationEventHandler>();

            IEventBusSubscriber eventBus = ApplicationContainer.Resolve<RabbitMQEventBus>();
            eventBus.Subscribe<OrderCreatedIntegrationEvent, OrderCreatedIntegrationEventHandler>();

            //IEventBusSubscriber storeAndForwardEventBus = ApplicationContainer.Resolve<StoreAndForwardEventBusForwarder>();

            //storeAndForwardEventBus.Subscribe<ForwardIntegrationEventHandler>(nameof(OrderCreatedIntegrationEvent));

            app.UseMvc();
        }
    }
}
