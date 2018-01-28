using Axerrio.BB.DDD.Infrastructure.IntegrationEvents.Abstractions;
using EnsureThat;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Axerrio.BB.DDD.Infrastructure.IntegrationEvents
{
    public class EventBusPublisherFactory : IEventBusPublisherFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public EventBusPublisherFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = EnsureArg.IsNotNull(serviceProvider, nameof(serviceProvider));
        }

        public IEventBusPublisher Create<TEventBus>() where TEventBus : IEventBusPublisher
        {
            return _serviceProvider.GetRequiredService<TEventBus>();
        }
    }
}
