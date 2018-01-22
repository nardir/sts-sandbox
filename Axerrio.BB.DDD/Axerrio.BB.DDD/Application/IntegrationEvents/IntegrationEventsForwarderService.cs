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
        private readonly IIntegrationEventsDequeueService _integrationEventsDequeueService;
        private readonly ILogger<IntegrationEventsForwarderService<TEventBus>> _logger;

        public IntegrationEventsForwarderService(IEventBusPublishOnlyFactory eventBusPublishOnlyFactory 
            , IIntegrationEventsDequeueService integrationEventsDequeueService
            , ILogger<IntegrationEventsForwarderService<TEventBus>> logger)
        {
            _logger = EnsureArg.IsNotNull(logger, nameof(logger));

            EnsureArg.IsNotNull(eventBusPublishOnlyFactory, nameof(eventBusPublishOnlyFactory));

            _eventBus = eventBusPublishOnlyFactory.Create<TEventBus>();

            _integrationEventsDequeueService = EnsureArg.IsNotNull(integrationEventsDequeueService, nameof(integrationEventsDequeueService));
        }

        public async Task ForwardAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            bool cancel = false;
            var batchId = Guid.NewGuid();

            try
            {
                _logger.LogDebug($"Forwarding integration events for batch {batchId}");

                var requeuedItems = Enumerable.Empty<IntegrationEventsQueueItem>();
                var dequeuedItems = await _integrationEventsDequeueService.DequeueEventsAsync(batchId, cancellationToken);

                foreach (var eventQueueItem in dequeuedItems)
                {
                    try
                    {
                        if (cancellationToken.IsCancellationRequested)
                        {
                            cancel = true;

                            break;
                        }

                        _logger.LogDebug($"Forwarding integration event for queue item {eventQueueItem.EventQueueItemId}");

                        await _eventBus.PublishAsync(eventQueueItem.IntegrationEvent);

                        await _integrationEventsDequeueService.PublishEventAsync(eventQueueItem);

                        _logger.LogDebug($"Forwarded integration event for queue item {eventQueueItem.EventQueueItemId}");
                    }
                    catch (Exception exception)
                    {
                        _logger.LogError(exception, $"Exception while forwarding integration event for queue item {eventQueueItem.EventQueueItemId}");

                        await _integrationEventsDequeueService.RequeueOrFailEventAsync(eventQueueItem);
                    }
                }

                if (cancel)
                {
                    _logger.LogDebug($"Forwarding integration events cancelled for batch {batchId}");

                    //https://stackoverflow.com/questions/36426937/what-is-the-difference-between-wait-vs-getawaiter-getresult
                    //Mark all events with forward/dequeue batch id as NotPublished
                    //In case of cancel the reenqueue needs to finish
                    requeuedItems = _integrationEventsDequeueService.RequeueEventsForBatchAsync(batchId).GetAwaiter().GetResult(); //In case of cancel the reenqueue needs to finish. GetAwaiter().GetResult() is used because it rearranges the stack trace in case of exception
                }

                _logger.LogDebug($"Forwarded integration events for batch {batchId} published #items {dequeuedItems.Count() - requeuedItems.Count()} requeued #items {requeuedItems.Count()}");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"Exception {exception.GetType().Name} with message ${exception.Message} detected while forwarding integration events for batch {batchId}");
            }
        }
    }
}
