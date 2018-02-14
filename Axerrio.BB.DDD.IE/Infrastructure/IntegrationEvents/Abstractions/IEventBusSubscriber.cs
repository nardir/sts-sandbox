using System;
using System.Collections.Generic;
using System.Text;

namespace Axerrio.BB.DDD.Infrastructure.IntegrationEvents.Abstractions
{
    public interface IEventBusSubscriber
    {
        void Subscribe<TIntegrationEvent, TIntegrationEventHandler>()
            where TIntegrationEvent : IntegrationEvent
            where TIntegrationEventHandler : IIntegrationEventHandler<TIntegrationEvent>;

        void Subscribe<TIntegrationEventHandler>(string eventName)
            where TIntegrationEventHandler : IIntegrationEventHandler;

        void Unsubscribe<TIntegrationEventHandler>(string eventName)
            where TIntegrationEventHandler : IIntegrationEventHandler;

        void Unsubscribe<TIntegrationEvent, TIntegrationEventHandler>()
            where TIntegrationEvent : IntegrationEvent
            where TIntegrationEventHandler : IIntegrationEventHandler<TIntegrationEvent>;
    }
}