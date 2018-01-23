using Axerrio.BB.DDD.Application.IntegrationEvents.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.BB.DDD.Application.IntegrationEvents
{
    public class InMemoryEventBusSubscriptionsManager : IEventBusSubscriptionsManager
    {
        private readonly Dictionary<string, List<IntegrationEventsSubscription>> _eventHandlers;
        private readonly List<Type> _eventTypes;

        public InMemoryEventBusSubscriptionsManager()
        {
            _eventHandlers = new Dictionary<string, List<IntegrationEventsSubscription>>();
            _eventTypes = new List<Type>();
        }

        #region IEventBusSubscriptionsManager

        public event EventHandler<string> OnEventRemoved;

        public bool IsEmpty => !_eventHandlers.Keys.Any();

        public void Clear()
        {
            _eventHandlers.Clear();
            _eventTypes.Clear();
        }

        public Type GetEventTypeByName(string eventName) => _eventTypes.SingleOrDefault(t => t.Name == eventName);

        public string GetEventName<TIntegrationEvent>() where TIntegrationEvent : IntegrationEvent
        {
            return typeof(TIntegrationEvent).Name;
        }

        public void AddSubscription<TIntegrationEventHandler>(string eventName) where TIntegrationEventHandler : IDynamicIntegrationEventHandler
        {
            AddSubscription(typeof(TIntegrationEventHandler), eventName, isDynamicHandler: true);
        }

        public void AddSubscription<TIntegrationEvent, TIntegrationEventHandler>()
            where TIntegrationEvent : IntegrationEvent
            where TIntegrationEventHandler : IIntegrationEventHandler<TIntegrationEvent>
        {
            var eventName = GetEventName<TIntegrationEvent>();

            AddSubscription(typeof(TIntegrationEventHandler), eventName, isDynamicHandler: false);

            _eventTypes.Add(typeof(TIntegrationEvent));
        }

        private void AddSubscription(Type handlerType, string eventName, bool isDynamicHandler)
        {
            if (!HasSubscriptionsForEvent(eventName))
            {
                _eventHandlers.Add(eventName, new List<IntegrationEventsSubscription>());
            }

            if (_eventHandlers[eventName].Any(s => s.HandlerType == handlerType))
            {
                throw new ArgumentException($"Handler Type {handlerType.Name} already registered for '{eventName}'", nameof(handlerType));
            }

            _eventHandlers[eventName].Add(IntegrationEventsSubscription.Create(isDynamicHandler, handlerType));
        }

        public IEnumerable<IntegrationEventsSubscription> GetHandlersForEvent<TIntegrationEvent>() where TIntegrationEvent : IntegrationEvent
        {
            var eventName = GetEventName<TIntegrationEvent>();

            return GetHandlersForEvent(eventName);
        }

        public IEnumerable<IntegrationEventsSubscription> GetHandlersForEvent(string eventName) => _eventHandlers[eventName];

        public bool HasSubscriptionsForEvent<TIntegrationEvent>() where TIntegrationEvent : IntegrationEvent
        {
            var eventName = GetEventName<TIntegrationEvent>();

            return HasSubscriptionsForEvent(eventName);
        }

        public bool HasSubscriptionsForEvent(string eventName) => _eventHandlers.ContainsKey(eventName);

        public void RemoveSubscription<TIntegrationEventHandler>(string eventName) where TIntegrationEventHandler : IDynamicIntegrationEventHandler
        {
            var subscription = FindDynamicSubscriptionToRemove<TIntegrationEventHandler>(eventName);

            RemoveSubscription(eventName, subscription);
        }

        public void RemoveSubscription<TIntegrationEvent, TIntegrationEventHandler>()
            where TIntegrationEvent : IntegrationEvent
            where TIntegrationEventHandler : IIntegrationEventHandler<TIntegrationEvent>
        {
            var subscription = FindSubscriptionToRemove<TIntegrationEvent, TIntegrationEventHandler>();

            var eventName = GetEventName<TIntegrationEvent>();

            RemoveSubscription(eventName, subscription);
        }

        private void RemoveSubscription(string eventName, IntegrationEventsSubscription subscription)
        {
            if (subscription != null)
            {
                _eventHandlers[eventName].Remove(subscription);

                if (!_eventHandlers[eventName].Any())
                {
                    _eventHandlers.Remove(eventName);

                    var eventType = _eventTypes.SingleOrDefault(e => e.Name == eventName);

                    if (eventType != null)
                    {
                        _eventTypes.Remove(eventType);
                    }

                    RaiseOnEventRemoved(eventName);
                }
            }
        }

        #endregion

        private IntegrationEventsSubscription FindDynamicSubscriptionToRemove<TIntegrationEventHandler>(string eventName)
            where TIntegrationEventHandler : IDynamicIntegrationEventHandler
        {
            return FindSubscriptionToRemove(eventName, typeof(TIntegrationEventHandler));
        }


        private IntegrationEventsSubscription FindSubscriptionToRemove<TIntegrationEvent, TIntegrationEvenetHandler>()
             where TIntegrationEvent : IntegrationEvent
             where TIntegrationEvenetHandler : IIntegrationEventHandler<TIntegrationEvent>
        {
            var eventName = GetEventName<TIntegrationEvent>();

            return FindSubscriptionToRemove(eventName, typeof(TIntegrationEvenetHandler));
        }

        private IntegrationEventsSubscription FindSubscriptionToRemove(string eventName, Type handlerType)
        {
            if (!HasSubscriptionsForEvent(eventName))
            {
                return null;
            }

            return _eventHandlers[eventName].SingleOrDefault(s => s.HandlerType == handlerType);

        }

        private void RaiseOnEventRemoved(string eventName)
        {
            var handler = OnEventRemoved; //Notice: handler is a C# event handler not an integration event handler
            if (handler != null)
            {
                OnEventRemoved(this, eventName);
            }
        }
    }
}
