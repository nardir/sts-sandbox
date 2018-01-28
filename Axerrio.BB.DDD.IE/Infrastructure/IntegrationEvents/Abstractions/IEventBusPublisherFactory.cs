using System;
using System.Collections.Generic;
using System.Text;

namespace Axerrio.BB.DDD.Infrastructure.IntegrationEvents.Abstractions
{
    public interface IEventBusPublisherFactory
    {
        IEventBusPublisher Create<TEventBus>() where TEventBus : IEventBusPublisher;
    }
}
