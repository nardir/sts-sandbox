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
            _logger = EnsureArg.IsNotNull(logger, nameof(logger));

            EnsureArg.IsNotNull(eventBusPublishOnlyFactory, nameof(eventBusPublishOnlyFactory));

            //_eventBus = eventBusPublishOnlyFactory.Create<IEventBus>();
            _eventBus = eventBusPublishOnlyFactory.Create<TEventBus>();

            _integrationEventsQueueService = EnsureArg.IsNotNull(integrationEventsQueueService, nameof(integrationEventsQueueService));
        }

        public async Task ForwardAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            bool cancel = false;

            //Dequeue Store
            //Meegeven Forward batch id (GUID?) als Dequeue ID
            var events = await _integrationEventsQueueService.DequeueEventsAsync(cancellationToken: cancellationToken);

            foreach (var @event in events)
            {
                try
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        cancel = true;

                        break;
                    }

                    //Publish eventbus
                    _eventBus.Publish(@event);

                    //MarkAsPublished store
                    //await _integrationEventsQueueService.MarkEventAsPublishedAsync(@event, cancellationToken);
                    await _integrationEventsQueueService.MarkEventAsPublishedAsync(@event);
                }
                catch (Exception ex)
                {
                    //Exception:
                    //If max retries bereikt dan MarkAsPublishedFailed
                    //else MarkAsNotPublished
                    //await _integrationEventsQueueService.MarkEventAsNotPublishedAsync(@event, cancellationToken);
                    await _integrationEventsQueueService.MarkEventAsNotPublishedAsync(@event);
                }
            }

            if (cancel)
            {
                //Mark all events with forward/dequeue batch id as NotPublished
            }
        }
    }
}
