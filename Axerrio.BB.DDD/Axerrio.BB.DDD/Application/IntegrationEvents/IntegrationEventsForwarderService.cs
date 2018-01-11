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
    public class IntegrationEventsForwarderService<TEventBus> : IIntegrationEventsForwarderService
        where TEventBus: IEventBusPublishOnly
    {
        private readonly IEventBusPublishOnly _eventBus;
        private readonly IIntegrationEventsQueueService _integrationEventsQueueService;
        private readonly ILogger<IntegrationEventsForwarderService<TEventBus>> _logger;

        public IntegrationEventsForwarderService(IEventBusPublishOnlyFactory eventBusPublishOnlyFactory 
            , IIntegrationEventsQueueService integrationEventsQueueService
            , ILogger<IntegrationEventsForwarderService<TEventBus>> logger)
        {
            EnsureArg.IsNotNull(eventBusPublishOnlyFactory, nameof(eventBusPublishOnlyFactory));

            _logger = EnsureArg.IsNotNull(logger, nameof(logger));

            //_eventBus = eventBusPublishOnlyFactory.Create<IEventBus>();
            _eventBus = eventBusPublishOnlyFactory.Create<TEventBus>();
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
