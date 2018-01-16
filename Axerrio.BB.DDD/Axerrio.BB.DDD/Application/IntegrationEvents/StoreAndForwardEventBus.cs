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
        private readonly IIntegrationEventsQueueService _integrationEventsQueueService;

        public StoreAndForwardEventBus(IIntegrationEventsQueueService integrationEventsQueueService)
        {
            _integrationEventsQueueService = EnsureArg.IsNotNull(integrationEventsQueueService, nameof(integrationEventsQueueService));
        }

        public async Task PublishAsync(IntegrationEvent @event, CancellationToken cancellationToken = default(CancellationToken))
        {
            //_integrationEventsQueueService.EnqueueEvent(@event);

            EnsureArg.IsNotNull(@event, nameof(@event));

            await _integrationEventsQueueService.EnqueueEventAsync(new IntegrationEventsQueueItem(@event));
        }
    }
}