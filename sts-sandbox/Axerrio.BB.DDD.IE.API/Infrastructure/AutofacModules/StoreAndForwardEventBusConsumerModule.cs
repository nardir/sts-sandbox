using Autofac;
using Axerrio.BB.DDD.EntityFrameworkCore.Infrastructure.IntegrationEvents;
using Axerrio.BB.DDD.Infrastructure.IntegrationEvents;
using Axerrio.BB.DDD.Infrastructure.IntegrationEvents.Abstractions;
using Axerrio.BB.DDD.Infrastructure.Scheduling;
using Microsoft.Extensions.Hosting;
using Quartz.Spi;

namespace Axerrio.BB.DDD.IE.API.Infrastructure.AutofacModules
{
    public class StoreAndForwardEventBusConsumerModule : Autofac.Module
      
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<StoreAndForwardEventBusForwarder>()
               .As<IEventBusForwarder>()
               .AsSelf()
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
        }
    }
}

