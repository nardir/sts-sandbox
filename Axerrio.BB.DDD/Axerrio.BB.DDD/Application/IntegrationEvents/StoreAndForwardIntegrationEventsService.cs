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
    public class StoreAndForwardIntegrationEventsService : IIntegrationEventsService
    {
        private readonly ILogger<StoreAndForwardIntegrationEventsService> _logger;
        private readonly IEventBusStoreAndForward _eventBus;

        public StoreAndForwardIntegrationEventsService(IEventBusStoreAndForward eventBus, ILogger<StoreAndForwardIntegrationEventsService> logger)
        {
            _logger = EnsureArg.IsNotNull(logger, nameof(logger));
            _eventBus = EnsureArg.IsNotNull(eventBus, nameof(eventBus));
        }

        public void Publish(IntegrationEvent @event)
        {
            //TODO NR : Logging

            _eventBus.Publish(@event);

            //return _integrationEventsEnqueueService.EnqueueAsync(@event, cancellationToken);
        }
    }
}
