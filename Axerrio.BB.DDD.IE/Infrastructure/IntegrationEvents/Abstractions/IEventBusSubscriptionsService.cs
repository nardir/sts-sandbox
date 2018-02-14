using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Axerrio.BB.DDD.Infrastructure.IntegrationEvents.Abstractions
{
    public interface IEventBusSubscriptionsService
    {
        bool IsEmpty { get; }
        void Clear();

        event EventHandler<string> EventRemoved;
        event EventHandler<string> EventAdded;

        Type GetEventTypeByName(string eventName);

        IEnumerable<string> GetEvents();

        void AddSubscription<TIntegrationEventHandler>(string eventName)
            where TIntegrationEventHandler : IIntegrationEventHandler;

        void AddSubscription<TIntegrationEvent, TIntegrationEventHandler>()
           where TIntegrationEvent : IntegrationEvent
           where TIntegrationEventHandler : IIntegrationEventHandler<TIntegrationEvent>;

        void RemoveSubscription<TIntegrationEventHandler>(string eventName)
            where TIntegrationEventHandler : IIntegrationEventHandler;

        void RemoveSubscription<TIntegrationEvent, TIntegrationEventHandler>()
             where TIntegrationEventHandler : IIntegrationEventHandler<TIntegrationEvent>
             where TIntegrationEvent : IntegrationEvent;

        IEnumerable<EventBusSubscription> GetHandlersForEvent<TIntegrationEvent>() where TIntegrationEvent : IntegrationEvent;
        IEnumerable<EventBusSubscription> GetHandlersForEvent(string eventName);

        bool HasSubscriptionsForEvent<TIntegrationEvent>() where TIntegrationEvent : IntegrationEvent;
        bool HasSubscriptionsForEvent(string eventName);

        Task DispatchEventAsync(string eventName, string eventMessage, CancellationToken cancellationToken = default(CancellationToken));
    }
}