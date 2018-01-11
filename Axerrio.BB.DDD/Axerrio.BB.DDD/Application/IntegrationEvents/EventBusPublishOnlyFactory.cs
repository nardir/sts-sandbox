using Axerrio.BB.DDD.Application.IntegrationEvents.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using EnsureThat;

namespace Axerrio.BB.DDD.Application.IntegrationEvents
{
    public class EventBusPublishOnlyFactory : IEventBusPublishOnlyFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public EventBusPublishOnlyFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = EnsureArg.IsNotNull(serviceProvider, nameof(serviceProvider));
        }
        public IEventBusPublishOnly Create<TEventBus>() where TEventBus : IEventBusPublishOnly
        {
            return _serviceProvider.GetRequiredService<TEventBus>();
        }
    }
}
