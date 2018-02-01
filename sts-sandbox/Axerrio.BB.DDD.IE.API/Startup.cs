using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Axerrio.BB.DDD.EntityFrameworkCore.Infrastructure;
using Axerrio.BB.DDD.EntityFrameworkCore.Infrastructure.IntegrationEvents;
using Axerrio.BB.DDD.EntityFrameworkCore.Infrastructure.IntegrationEvents.Autofac;
using Axerrio.BB.DDD.IE.API.Application;
using Axerrio.BB.DDD.Infrastructure.IntegrationEvents;
using Axerrio.BB.DDD.Infrastructure.IntegrationEvents.Abstractions;
using Axerrio.BB.AspNetCore.Extensions.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Axerrio.BB.DDD.IE.RabbitMQ.Infrastructure;
using Microsoft.Extensions.Hosting;
using Axerrio.BB.DDD.Infrastructure.Scheduling;
using Quartz.Spi;

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
                options.TriggerIntervalInMilliseconds = 500;
            });

            services.Configure<StoreAndForwardEventBusForwardOptions>(configuration: Configuration, key: "StoreAndForwardEventBusForwardOptions");
            services.Configure<StoreAndForwardEventBusForwardOptions>(options => 
            {
                options.MaxPublishAttempts = 3;
            });

            services.Configure<EventBusOptions>(configuration: Configuration, key: "RabbitMQEventBus");

            services.AddEntityFrameworkSqlServer()
                .AddDbContext<OrderingDbContext>(options =>
                {
                    options.UseSqlServer(connectionString, sqlOptions =>
                    {
                        sqlOptions.MigrationsAssembly(typeof(OrderingDbContext).GetTypeInfo().Assembly.GetName().Name);
                    });
                });

            //services.AddTransient<EFCoreStoreAndForwardEventBus<OrderingDbContext>>();

            services.AddMvc();

            //http://autofaccn.readthedocs.io/en/latest/integration/aspnetcore.html
            var builder = new ContainerBuilder();

            builder.Populate(services);

            //builder.RegisterModule(new EFCoreIntegrationEventsModule<OrderingDbContext>());

            #region IntegrationEventHandlers

            builder.RegisterAssemblyTypes(typeof(Startup).GetTypeInfo().Assembly)
                .AsClosedTypesOf(typeof(IIntegrationEventHandler<>));

            #endregion

            #region StoreAndForwardEventBusPublisher

            builder.RegisterType<StoreAndForwardEventBusPublisher<OrderingDbContext>>()
                .InstancePerLifetimeScope();

            builder.RegisterType<IntegrationEventsService<StoreAndForwardEventBusPublisher<OrderingDbContext>>>()
                .As<IIntegrationEventsService>()
                .InstancePerLifetimeScope();

            #endregion

            #region RabbitMQEventBus

            builder.RegisterType<InMemoryEventBusSubscriptionsService>()
                .As<IEventBusSubscriptionsService>()
                .SingleInstance();

            builder.RegisterType<RabbitMQEventBus>()
                .UsingConstructor(typeof(IEventBusSubscriptionsService), typeof(IOptions<EventBusOptions>), typeof(ILogger<RabbitMQEventBus>))
                .As<IEventBusPublisher>()
                .As<IEventBusConsumer>()
                .AsSelf()
                .SingleInstance();

            builder.RegisterType<EventBusConsumerHostedService<RabbitMQEventBus>>()
                .As<IHostedService>()
                .SingleInstance();

            #endregion

            #region StoreAndForwardEventBusConsumer

            builder.RegisterType<StoreAndForwardEventBusForwarder>()
                .As<IEventBusForwarder>()
                .SingleInstance();

            builder.RegisterType<StoreAndForwardEventBusConsumerTriggerFactory>()
                .InstancePerLifetimeScope();

            builder.RegisterType<JobFactory>()
                .As<IJobFactory>()
                .InstancePerLifetimeScope();

            builder.RegisterType<StoreAndForwardEventBusConsumerJob>()
                .InstancePerLifetimeScope();

            builder.RegisterType<StoreAndForwardEventBusConsumer<StoreAndForwardEventBusConsumerTriggerFactory, StoreAndForwardEventBusConsumerJob>>()
                .SingleInstance();

            builder.RegisterType<EventBusConsumerHostedService<StoreAndForwardEventBusConsumer<StoreAndForwardEventBusConsumerTriggerFactory, StoreAndForwardEventBusConsumerJob>>>()
                .As<IHostedService>()
                .SingleInstance();

            #endregion

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

            var subscriptionsService = app.ApplicationServices.GetRequiredService<IEventBusSubscriptionsService>();

            subscriptionsService.AddSubscription<OrderCreatedIntegrationEvent, OrderCreatedIntegrationEventHandler>();

            app.UseMvc();
        }
    }
}
