using Axerrio.BB.DDD.Application.IntegrationEvents.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.BB.DDD.Application.IntegrationEvents
{
    public class StoreAndForwardEventBus : IEventBusStoreAndForward
    {
        private readonly IIntegrationEventsQueueService _integrationEventsQueueService;

        public StoreAndForwardEventBus(IIntegrationEventsQueueService integrationEventsQueueService)
        {

        }

        public void Publish(IntegrationEvent @event)
        {
            _integrationEventsQueueService.EnqueueEvent(@event);
        }
    }
}
