using Autofac;
using Axerrio.BB.DDD.Infrastructure.IntegrationEvents;
using Axerrio.BB.DDD.Infrastructure.IntegrationEvents.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Axerrio.BB.DDD.IE.API.Infrastructure.AutofacModules
{
    public class EventBusModule<TEventBus> : Autofac.Module
        where TEventBus: IEventBusConsumer
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<InMemoryEventBusSubscriptionsService>()
                .As<IEventBusSubscriptionsService>();
            //.SingleInstance();

            builder.RegisterType<TEventBus>()
                .UsingConstructor(typeof(IEventBusSubscriptionsService), typeof(IOptions<EventBusOptions>), typeof(ILogger<TEventBus>))
                .As<IEventBusPublisher>()
                .As<IEventBusConsumer>()
                //.As<IEventBusSubscriber>()
                .AsSelf()
                .SingleInstance();

            builder.RegisterType<EventBusConsumerHostedService<TEventBus>>()
                .As<IHostedService>()
                .SingleInstance();
        }
    }
}

