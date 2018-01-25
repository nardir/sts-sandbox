using Autofac;
using Axerrio.BB.DDD.Application.IntegrationEvents.Abstractions;
using EnsureThat;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

        private readonly ILifetimeScope _lifetimeScope;
        //private readonly string AUTOFAC_SCOPE_NAME = "eshop_event_bus";

        public InMemoryEventBusSubscriptionsManager(ILifetimeScope lifetimeScope)
        {
            _lifetimeScope = EnsureArg.IsNotNull(lifetimeScope, nameof(lifetimeScope));

            _eventHandlers = new Dictionary<string, List<IntegrationEventsSubscription>>();
            _eventTypes = new List<Type>();
        }

        #region IEventBusSubscriptionsManager

        public event EventHandler<string> EventRemoved;
        public event EventHandler<string> EventAdded;

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

        public IEnumerable<string> Events => _eventHandlers.Keys;

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

            if (!_eventTypes.Contains(typeof(TIntegrationEvent)))
                _eventTypes.Add(typeof(TIntegrationEvent));
        }

        private void AddSubscription(Type handlerType, string eventName, bool isDynamicHandler)
        {
            if (!HasSubscriptionsForEvent(eventName))
            {
                _eventHandlers.Add(eventName, new List<IntegrationEventsSubscription>());

                OnEventAdded(eventName);
            }

            if (_eventHandlers[eventName].Any(s => s.HandlerType == handlerType))
            {
                throw new ArgumentException($"Handler Type {handlerType.Name} already registered for '{eventName}'", nameof(handlerType));
            }

            _eventHandlers[eventName].Add(IntegrationEventsSubscription.Create(isDynamicHandler, handlerType));
        }

        private void OnEventAdded(string eventName)
        {
            var handler = EventAdded; //Notice: handler is a C# event handler not an integration event handler

            handler?.Invoke(this, eventName);

            //if (handler != null)
            //{
            //    //EventRemoved(this, eventName);
            //    handler.Invoke(this, eventName);
            //}
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

                    OnEventRemoved(eventName);
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

        private void OnEventRemoved(string eventName)
        {
            var handler = EventRemoved; //Notice: handler is a C# event handler not an integration event handler

            handler?.Invoke(this, eventName);

            //if (handler != null)
            //{
            //    //EventRemoved(this, eventName);
            //    handler.Invoke(this, eventName);
            //}
        }

        public async Task DispatchEventAsync(string eventName, string eventMessage)
        {
            if (HasSubscriptionsForEvent(eventName))
            {
                using (var scope = _lifetimeScope.BeginLifetimeScope())
                {
                    var subscriptions = GetHandlersForEvent(eventName);
                    foreach (var subscription in subscriptions)
                    {
                        if (subscription.IsDynamicHandler)
                        {
                            var eventHandler = scope.ResolveOptional(subscription.HandlerType) as IDynamicIntegrationEventHandler;

                            dynamic @event = JObject.Parse(eventMessage);

                            await eventHandler.HandleAsync(@event);
                        }
                        else
                        {
                            var eventType = GetEventTypeByName(eventName);

                            var @event = JsonConvert.DeserializeObject(eventMessage, eventType);

                            var eventHandler = scope.ResolveOptional(subscription.HandlerType);
                            var closedEventHandlerType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);

                            await (Task) closedEventHandlerType.GetMethod("HandleAsync").Invoke(eventHandler, new object[] { @event });
                        }
                    }
                }
            }
        }
    }
}
