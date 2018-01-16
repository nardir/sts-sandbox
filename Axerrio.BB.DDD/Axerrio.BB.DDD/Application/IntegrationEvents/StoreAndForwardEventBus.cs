using Axerrio.BB.DDD.Application.IntegrationEvents.Abstractions;
using EnsureThat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Axerrio.BB.DDD.Application.IntegrationEvents
{
    public class StoreAndForwardEventBus : IEventBusPublishOnly
    {
        private readonly IIntegrationEventsEnqueueService _integrationEventsEnqueueService;

        public StoreAndForwardEventBus(IIntegrationEventsEnqueueService integrationEventsEnqueueService)
        {
            _integrationEventsEnqueueService = EnsureArg.IsNotNull(integrationEventsEnqueueService, nameof(integrationEventsEnqueueService));
        }

        public async Task PublishAsync(IntegrationEvent @event, CancellationToken cancellationToken = default(CancellationToken))
        {
            //_integrationEventsQueueService.EnqueueEvent(@event);

            EnsureArg.IsNotNull(@event, nameof(@event));

            await _integrationEventsEnqueueService.EnqueueEventAsync(new IntegrationEventsQueueItem(@event));
        }
    }
}