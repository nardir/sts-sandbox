using Autofac;
using Autofac.Extensions.DependencyInjection;
using Axerrio.BB.DDD.Application.IntegrationEvents;
using Axerrio.BB.DDD.Application.IntegrationEvents.Abstractions;
using Axerrio.BB.DDD.EntityFrameworkCore.Infrastructure.IntegrationEvents;
using Axerrio.BB.DDD.EntityFrameworkCore.Infrastructure.IntegrationEvents.Extensions;
using Axerrio.BB.DDD.Infrastructure.Hosting;
using Axerrio.BB.DDD.Infrastructure.IntegrationEvents;
using Axerrio.BB.DDD.Infrastructure.IntegrationEvents.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Quartz.Spi;
using RabbitMQ.Client;
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

        public IContainer ApplicationContainer { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container.

        ///public void ConfigureServices(IServiceCollection services)
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            var connectionString = Configuration.GetValue<string>("ConnectionString");

            services.AddOptions();

            services.AddEntityFrameworkSqlServer()
                .AddDbContext<OrderingDbContext>(options => 
            {
                options.UseSqlServer(connectionString, sqlOptions => 
                {
                    sqlOptions.MigrationsAssembly(typeof(OrderingDbContext).GetTypeInfo().Assembly.GetName().Name);
                });
            });

            //services.AddTransient<PaymentMethodCreatedIntegrationEventHandler>();
            //services.AddTransient<PaymentMethodCreatedIntegrationEventHandlerSecond>();
            //services.AddTransient<IIntegrationEventHandler<PaymentMethodCreatedIntegrationEvent>, PaymentMethodCreatedIntegrationEventHandler>();

            services.Configure<RabbitMQEventBusOptions>(options => 
            {
                options.ConnectRetryAttempts = 3;

                options.Exchange = "AFO-Test-Exchange";
                options.QueueName = "AFO-Test-Queue";

                options.PublishRetryAttempts = 2;

                //"amqp://user:pass@hostName:port/vhost"
                options.ConnectionString = @"amqp://user:password@localhost:8081//";
            });

            services.AddSingleton<IRabbitMQPersistentConnection>(p =>
            {
                var options = p.GetRequiredService<IOptions<RabbitMQEventBusOptions>>().Value;

                var factory = new ConnectionFactory()
                {
                    Uri = new Uri(options.ConnectionString),
                };

                var logger = p.GetRequiredService<ILogger<DefaultRabbitMQPersistentConnection>>();

                return new DefaultRabbitMQPersistentConnection(factory, logger, options.ConnectRetryAttempts);
            });

            services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();

            services.AddSingleton<RabbitMQEventBus>();

            services.AddSingleton<FileEventBus>();
            services.AddSingleton<IEventBus>(provider =>
            {
                return provider.GetRequiredService<RabbitMQEventBus>();
            });

            //services.AddEFCoreStoreAndForwardIntegrationEventsServices<OrderingDbContext, FileEventBus>(connectionString, Configuration);
            services.AddEFCoreStoreAndForwardIntegrationEventsServices<OrderingDbContext>(connectionString, Configuration);
            //services.AddEFCoreStoreAndForwardIntegrationEventsServices<OrderingDbContext, RabbitMQEventBus>(connectionString, Configuration);


            //services.AddSingleton<IHostedService, TestHostedService>();
            //services.AddSingleton<IHostedService, TestHostedService>(p =>
            //{
            //    using (var scope = p.CreateScope())
            //    {
            //        var context = scope.ServiceProvider.GetService<OrderingDbContext>();
            //        var logger = p.GetService<ILogger<TestHostedService>>();

            //        return new TestHostedService(logger, context);
            //    }
            //});

            //services.AddTransient<TestHostedServiceFactory>();
            //services.AddSingleton<IHostedService, TestHostedService>(p =>
            //{
            //    using (var scope = p.CreateScope())
            //    {
            //        var factory = scope.ServiceProvider.GetRequiredService<TestHostedServiceFactory>();

            //        return factory.Create();
            //    }
            //});

            //http://autofaccn.readthedocs.io/en/latest/integration/aspnetcore.html


            //var pm = new PaymentMethod(1, "VISA", "1234-4567-9999-1111", "123", "Piet", DateTime.UtcNow.AddYears(1));
            //var pmjson = JsonConvert.SerializeObject(pm);
            //var pmrestored = JsonConvert.DeserializeObject<PaymentMethod>(pmjson);

            //var ie = new PaymentMethodCreatedIntegrationEvent(pm);
            //var iejson = JsonConvert.SerializeObject(ie);
            //var ierestored = JsonConvert.DeserializeObject<PaymentMethodCreatedIntegrationEvent>(iejson);

            //var queueItem = new IntegrationEventsQueueItem(ierestored);
            //var iefromQueueItem = queueItem.IntegrationEvent;

            services.AddMvc()
                .AddControllersAsServices();

            //http://autofaccn.readthedocs.io/en/latest/integration/aspnetcore.html
            var builder = new ContainerBuilder();

            builder.Populate(services);
            //builder.RegisterType<MyType>().As<IMyType>();
            //builder.RegisterType<FileEventBus>().As<IEventBusPublishOnly>().UsingConstructor();

            //builder.RegisterType<TestHostedServiceFactory>();
            ////NR: Onderstaand is uiteindelijk de beste oplossing
            //builder.RegisterType<TestHostedService>().InstancePerLifetimeScope();
            //builder.Register<TestHostedService>(context =>
            //    {
            //        var factory = context.Resolve<TestHostedService.Factory>();

            //        //return factory.Invoke();
            //        return factory();

            //        //var factory = context.Resolve<TestHostedServiceFactory>();

            //        //return factory.Create();

            //        //return new TestHostedService(context.Resolve<ILogger<TestHostedService>>(), context.Resolve<OrderingDbContext>());
            //    })
            //    .As<IHostedService>()
            //    .SingleInstance();

            builder.RegisterAssemblyTypes(typeof(Startup).GetTypeInfo().Assembly)
                       .AsClosedTypesOf(typeof(IIntegrationEventHandler<>));

            builder.RegisterType<OrderCreatedIntegrationEventHandlerDynamic>();

            ApplicationContainer = builder.Build();

            //var scope = ApplicationContainer.BeginLifetimeScope("testhostedservice", b => 
            //{
            //    b.RegisterType<TestHostedService>();
            //    b.Register<TestHostedService>(context =>
            //    {
            //        var factory = context.Resolve<TestHostedService.Factory>();

            //        //return factory.Invoke();
            //        return factory();

            //        //var factory = context.Resolve<TestHostedServiceFactory>();

            //        //return factory.Create();

            //        //return new TestHostedService(context.Resolve<ILogger<TestHostedService>>(), context.Resolve<OrderingDbContext>());
            //    })
            //        .As<IHostedService>()
            //        .SingleInstance();
            //});

            // Create the IServiceProvider based on the container.
            return new AutofacServiceProvider(ApplicationContainer);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app
            , IHostingEnvironment env
            , IApplicationLifetime appLifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();

            appLifetime.ApplicationStopped.Register(() => ApplicationContainer.Dispose());

            var subscriptionsManager = app.ApplicationServices.GetRequiredService<IEventBusSubscriptionsManager>();

            subscriptionsManager.AddSubscription<PaymentMethodCreatedIntegrationEvent, PaymentMethodCreatedIntegrationEventHandler>();
            subscriptionsManager.AddSubscription<PaymentMethodCreatedIntegrationEvent, PaymentMethodCreatedIntegrationEventHandlerSecond>();
            subscriptionsManager.AddSubscription<OrderCreatedIntegrationEvent, OrderCreatedIntegrationEventHandler>();
            subscriptionsManager.AddSubscription<OrderCreatedIntegrationEventHandlerDynamic>(nameof(OrderCreatedIntegrationEvent));

            var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();

            //eventBus.Subscribe<PaymentMethodCreatedIntegrationEvent, PaymentMethodCreatedIntegrationEventHandler>();
            //eventBus.Subscribe<PaymentMethodCreatedIntegrationEvent, PaymentMethodCreatedIntegrationEventHandlerSecond>();
        }
    }
}
