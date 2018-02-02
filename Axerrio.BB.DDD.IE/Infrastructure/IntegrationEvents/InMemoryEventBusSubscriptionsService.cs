using Autofac;
using Axerrio.BB.DDD.Infrastructure.IntegrationEvents.Abstractions;
using EnsureThat;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Axerrio.BB.DDD.Infrastructure.IntegrationEvents
{
    public class InMemoryEventBusSubscriptionsService : IEventBusSubscriptionsService
    {
        private readonly ILogger<InMemoryEventBusSubscriptionsService> _logger;

        private readonly Dictionary<string, List<EventBusSubscription>> _eventHandlers;
        private readonly List<Type> _eventTypes;

        private readonly ILifetimeScope _lifetimeScope;

        public InMemoryEventBusSubscriptionsService(ILogger<InMemoryEventBusSubscriptionsService> logger
            , ILifetimeScope lifetimeScope)
        {
            _logger = EnsureArg.IsNotNull(logger, nameof(logger));
            _lifetimeScope = EnsureArg.IsNotNull(lifetimeScope, nameof(lifetimeScope));

            _eventHandlers = new Dictionary<string, List<EventBusSubscription>>();
            _eventTypes = new List<Type>();
        }

        #region IEventBusSubscriptionsService

        public bool IsEmpty => !_eventHandlers.Keys.Any();

        public event EventHandler<string> EventRemoved;
        public event EventHandler<string> EventAdded;

        public void AddSubscription<TIntegrationEventHandler>(string eventName) where TIntegrationEventHandler : IIntegrationEventHandler
        {
            AddSubscription(typeof(TIntegrationEventHandler), eventName, isDynamicHandler: true);
        }

        public void AddSubscription<TIntegrationEvent, TIntegrationEventHandler>()
            where TIntegrationEvent : IntegrationEvent
            where TIntegrationEventHandler : IIntegrationEventHandler<TIntegrationEvent>
        {
            var eventName = IntegrationEvent.GetEventName<TIntegrationEvent>();

            AddSubscription(typeof(TIntegrationEventHandler), eventName, isDynamicHandler: false);

            if (!_eventTypes.Contains(typeof(TIntegrationEvent)))
                _eventTypes.Add(typeof(TIntegrationEvent));
        }

        public void Clear()
        {
            _eventHandlers.Clear();
            _eventTypes.Clear();
        }

        public async Task DispatchEventAsync(string eventName, string eventMessage, CancellationToken cancellationToken = default(CancellationToken))
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
                            var eventHandler = scope.ResolveOptional(subscription.HandlerType) as IIntegrationEventHandler;

                            dynamic @event = JObject.Parse(eventMessage);

                            await eventHandler.HandleAsync(eventName, @event, cancellationToken);
                        }
                        else
                        {
                            var eventType = GetEventTypeByName(eventName);

                            var @event = JsonConvert.DeserializeObject(eventMessage, eventType);

                            var eventHandler = scope.ResolveOptional(subscription.HandlerType);
                            var closedEventHandlerType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);

                            await(Task) closedEventHandlerType.GetMethod("HandleAsync").Invoke(eventHandler, new object[] { @event, cancellationToken });
                        }
                    }
                }
            }
        }

        public IEnumerable<string> GetEvents() => _eventHandlers.Keys;

        public Type GetEventTypeByName(string eventName) => _eventTypes.SingleOrDefault(t => t.Name == eventName);

        public IEnumerable<EventBusSubscription> GetHandlersForEvent<TIntegrationEvent>() where TIntegrationEvent : IntegrationEvent
        {
            var eventName = IntegrationEvent.GetEventName<TIntegrationEvent>();

            return GetHandlersForEvent(eventName);
        }

        public IEnumerable<EventBusSubscription> GetHandlersForEvent(string eventName) => _eventHandlers[eventName];

        public void RemoveSubscription<TIntegrationEventHandler>(string eventName) where TIntegrationEventHandler : IIntegrationEventHandler
        {
            var subscription = FindDynamicSubscriptionToRemove<TIntegrationEventHandler>(eventName);

            RemoveSubscription(eventName, subscription);
        }

        public void RemoveSubscription<TIntegrationEvent, TIntegrationEventHandler>()
            where TIntegrationEvent : IntegrationEvent
            where TIntegrationEventHandler : IIntegrationEventHandler<TIntegrationEvent>
        {
            var subscription = FindSubscriptionToRemove<TIntegrationEvent, TIntegrationEventHandler>();

            var eventName = IntegrationEvent.GetEventName<TIntegrationEvent>();

            RemoveSubscription(eventName, subscription);
        }

        public bool HasSubscriptionsForEvent<TIntegrationEvent>() where TIntegrationEvent : IntegrationEvent
        {
            var eventName = IntegrationEvent.GetEventName<TIntegrationEvent>();

            return HasSubscriptionsForEvent(eventName);
        }

        public bool HasSubscriptionsForEvent(string eventName) => _eventHandlers.ContainsKey(eventName);

        #endregion

        private void AddSubscription(Type handlerType, string eventName, bool isDynamicHandler)
        {
            if (!HasSubscriptionsForEvent(eventName))
            {
                _eventHandlers.Add(eventName, new List<EventBusSubscription>());

                OnEventAdded(eventName);
            }

            if (_eventHandlers[eventName].Any(s => s.HandlerType == handlerType))
            {
                //Logger !!!!!

                throw new ArgumentException($"Handler Type {handlerType.Name} already registered for '{eventName}'", nameof(handlerType));
            }

            _eventHandlers[eventName].Add(EventBusSubscription.Create(isDynamicHandler, handlerType));
        }

        private void OnEventAdded(string eventName)
        {
            var handler = EventAdded; //Notice: handler is a C# event handler not an integration event handler

            handler?.Invoke(this, eventName);
        }

        private void OnEventRemoved(string eventName)
        {
            var handler = EventRemoved; //Notice: handler is a C# event handler not an integration event handler

            handler?.Invoke(this, eventName);
        }

        private void RemoveSubscription(string eventName, EventBusSubscription subscription)
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

        private EventBusSubscription FindDynamicSubscriptionToRemove<TIntegrationEventHandler>(string eventName)
            where TIntegrationEventHandler : IIntegrationEventHandler
        {
            return FindSubscriptionToRemove(eventName, typeof(TIntegrationEventHandler));
        }

        private EventBusSubscription FindSubscriptionToRemove<TIntegrationEvent, TIntegrationEvenetHandler>()
             where TIntegrationEvent : IntegrationEvent
             where TIntegrationEvenetHandler : IIntegrationEventHandler<TIntegrationEvent>
        {
            var eventName = IntegrationEvent.GetEventName<TIntegrationEvent>();

            return FindSubscriptionToRemove(eventName, typeof(TIntegrationEvenetHandler));
        }

        private EventBusSubscription FindSubscriptionToRemove(string eventName, Type handlerType)
        {
            if (!HasSubscriptionsForEvent(eventName))
            {
                return null;
            }

            return _eventHandlers[eventName].SingleOrDefault(s => s.HandlerType == handlerType);
        }

    }
}
