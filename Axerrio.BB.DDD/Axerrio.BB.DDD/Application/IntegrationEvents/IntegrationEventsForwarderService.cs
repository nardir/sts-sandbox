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
            var batchId = Guid.NewGuid();

            //Dequeue Store
            //Meegeven Forward batch id (GUID?) als Dequeue ID
            var eventQueueItems = await _integrationEventsQueueService.DequeueEventsAsync(batchId, cancellationToken);

            foreach (var eventQueueItem in eventQueueItems)
            {
                try
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        cancel = true;

                        break;
                    }

                    //Publish eventbus
                    await _eventBus.PublishAsync(eventQueueItem.IntegrationEvent);

                    //MarkAsPublished store
                    //await _integrationEventsQueueService.MarkEventAsPublishedAsync(@event, cancellationToken);
                    await _integrationEventsQueueService.MarkEventAsPublishedAsync(eventQueueItem);
                }
                catch (Exception ex)
                {
                    //Exception:
                    //If max retries bereikt dan MarkAsPublishedFailed
                    //else MarkAsNotPublished
                    //await _integrationEventsQueueService.MarkEventAsNotPublishedAsync(@event, cancellationToken);
                    await _integrationEventsQueueService.MarkEventAsNotPublishedAsync(eventQueueItem);
                }
            }

            if (cancel)
            {
                //Mark all events with forward/dequeue batch id as NotPublished
                //In case of cancel the reenque needs to finish
                _integrationEventsQueueService.RequeueEventsForBatchAsync(batchId).GetAwaiter().GetResult(); //In case of cancel the reenqueue needs to finish. GetAwaiter().GetResult() is used because it rearranges the stack trace in case of exception
            }
        }
    }
}
