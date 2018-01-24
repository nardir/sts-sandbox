using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.BB.DDD.Application.IntegrationEvents.Abstractions
{
    public interface IEventBusSubscriptionsManager
    {
        bool IsEmpty { get; }

        event EventHandler<string> OnEventRemoved;

        void AddSubscription<TIntegrationEventHandler>(string eventName)
           where TIntegrationEventHandler : IDynamicIntegrationEventHandler;
        void AddSubscription<TIntegrationEvent, TIntegrationEventHandler>()
           where TIntegrationEvent : IntegrationEvent
           where TIntegrationEventHandler : IIntegrationEventHandler<TIntegrationEvent>;

        void RemoveSubscription<TIntegrationEventHandler>(string eventName)
            where TIntegrationEventHandler : IDynamicIntegrationEventHandler;
        void RemoveSubscription<TIntegrationEvent, TIntegrationEventHandler>()
             where TIntegrationEventHandler : IIntegrationEventHandler<TIntegrationEvent>
             where TIntegrationEvent : IntegrationEvent;

        bool HasSubscriptionsForEvent<TIntegrationEvent>() where TIntegrationEvent : IntegrationEvent;
        bool HasSubscriptionsForEvent(string eventName);

        Type GetEventTypeByName(string eventName);
        string GetEventName<TIntegrationEvent>() where TIntegrationEvent : IntegrationEvent;

        void Clear();

        IEnumerable<IntegrationEventsSubscription> GetHandlersForEvent<TIntegrationEvent>() where TIntegrationEvent : IntegrationEvent;
        IEnumerable<IntegrationEventsSubscription> GetHandlersForEvent(string eventName);

        Task DispatchEventAsync(string eventName, string eventMessage);
    }
}
