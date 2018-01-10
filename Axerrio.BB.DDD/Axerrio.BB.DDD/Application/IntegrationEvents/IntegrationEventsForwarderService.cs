using Axerrio.BB.DDD.Application.IntegrationEvents.Abstractions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Axerrio.BB.DDD.Application.IntegrationEvents
{
    public class IntegrationEventsForwarderService : IIntegrationEventsForwarderService
    {
        private readonly IEventBusPublishOnly _eventBus;
        private readonly IIntegrationEventsQueueService _integrationEventsQueueService;
        private readonly ILogger<IntegrationEventsForwarderService> _logger;

        public IntegrationEventsForwarderService(IEventBusPublishOnly eventBus
            , IIntegrationEventsQueueService integrationEventsQueueService
            , ILogger<IntegrationEventsForwarderService> logger)
        {

        }

        public async Task ForwardAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            //Dequeue Store
            var events = await _integrationEventsQueueService.DequeueEventsAsync(maxNumberOfEvents: 10, cancellationToken: cancellationToken);

            foreach (var @event in events)
            {
                try
                {
                    //Publish eventbus
                    _eventBus.Publish(@event);

                    //MarkAsPublished store
                    await _integrationEventsQueueService.MarkEventAsPublishedAsync(@event, cancellationToken);
                }
                catch (Exception ex)
                {
                    //Exception:
                    //If max retries bereikt dan MarkAsPublishedFailed
                    //else MarkAsNotPublished
                    await _integrationEventsQueueService.MarkEventAsNotPublishedAsync(@event, cancellationToken);
                }
            }
        }
    }
}
