using Axerrio.BB.DDD.Application.IntegrationEvents;
using Axerrio.BB.DDD.Application.IntegrationEvents.Abstractions;
using Axerrio.BB.DDD.EntityFrameworkCore.Infrastructure.IntegrationEvents;
using Axerrio.BB.DDD.EntityFrameworkCore.Infrastructure.IntegrationEvents.Extensions;
using Axerrio.BB.DDD.Infrastructure.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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

            services.AddEntityFrameworkSqlServer()
                .AddDbContext<OrderingDbContext>(options => 
            {
                options.UseSqlServer(connectionString, sqlOptions => 
                {
                    sqlOptions.MigrationsAssembly(typeof(OrderingDbContext).GetTypeInfo().Assembly.GetName().Name);
                });
            });

            services.AddSingleton<RabbitMQEventBus>();
            services.AddSingleton<FileEventBus>();
            services.AddSingleton<IEventBus>(provider =>
            {
                return provider.GetRequiredService<RabbitMQEventBus>();
            });

            services.AddEFCoreStoreAndForwardIntegrationEventsServices<OrderingDbContext, FileEventBus>(connectionString, Configuration);

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

            //http://autofaccn.readthedocs.io/en/latest/integration/aspnetcore.html


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
