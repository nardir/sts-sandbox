using Axerrio.BB.DDD.Infrastructure.IntegrationEvents.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Axerrio.BB.DDD.IE.RabbitMQ.Infrastructure
{
    public partial class RabbitMQEventBus: IEventBusSubscriber
    {
        private readonly IEventBusSubscriptionsService _eventBusSubscriptionsService;

        public void Subscribe<TIntegrationEvent, TIntegrationEventHandler>()
            where TIntegrationEvent : IntegrationEvent
            where TIntegrationEventHandler : IIntegrationEventHandler<TIntegrationEvent>
        {
            _eventBusSubscriptionsService.AddSubscription<TIntegrationEvent, TIntegrationEventHandler>();
        }

        public void Unsubscribe<TIntegrationEvent, TIntegrationEventHandler>()
            where TIntegrationEvent : IntegrationEvent
            where TIntegrationEventHandler : IIntegrationEventHandler<TIntegrationEvent>
        {
            _eventBusSubscriptionsService.RemoveSubscription<TIntegrationEvent, TIntegrationEventHandler>();
        }

        public void Subscribe<TIntegrationEventHandler>(string eventName) where TIntegrationEventHandler : IIntegrationEventHandler
        {
            _eventBusSubscriptionsService.AddSubscription<TIntegrationEventHandler>(eventName);
        }

        public void Unsubscribe<TIntegrationEventHandler>(string eventName) where TIntegrationEventHandler : IIntegrationEventHandler
        {
            _eventBusSubscriptionsService.RemoveSubscription<TIntegrationEventHandler>(eventName);
        }
    }
}
