using Axerrio.BB.DDD.Application.IntegrationEvents.Abstractions;
using EnsureThat;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Axerrio.BB.DDD.Application.IntegrationEvents
{
    public class StoreAndForwardIntegrationEventsService<TEventBus> : IIntegrationEventsService
        where TEventBus : IEventBusPublishOnly
    {
        private readonly ILogger<StoreAndForwardIntegrationEventsService<TEventBus>> _logger;
        //private readonly IEventBusStoreAndForward _eventBus;
        private readonly IEventBusPublishOnly _eventBus;

        //public StoreAndForwardIntegrationEventsService(IEventBusStoreAndForward eventBus, ILogger<StoreAndForwardIntegrationEventsService> logger)
        public StoreAndForwardIntegrationEventsService(IEventBusPublishOnlyFactory eventBusPublishOnlyFactory, ILogger<StoreAndForwardIntegrationEventsService<TEventBus>> logger)
        {
            var factory = EnsureArg.IsNotNull(eventBusPublishOnlyFactory, nameof(eventBusPublishOnlyFactory));

            _logger = EnsureArg.IsNotNull(logger, nameof(logger));


            _eventBus = factory.Create<TEventBus>();
        }

        public void Publish(IntegrationEvent @event)
        {
            //TODO NR : Logging

            _eventBus.Publish(@event);
        }
    }
}
