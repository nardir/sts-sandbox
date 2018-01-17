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

            //_eventBus = eventBusPublishOnlyFactory.Create<IEventBus>();
            _eventBus = eventBusPublishOnlyFactory.Create<TEventBus>();

            _integrationEventsDequeueService = EnsureArg.IsNotNull(integrationEventsDequeueService, nameof(integrationEventsDequeueService));
        }

        public async Task ForwardAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            bool cancel = false;
            var batchId = Guid.NewGuid();

            var eventQueueItems = await _integrationEventsDequeueService.DequeueEventsAsync(batchId, cancellationToken);

            foreach (var eventQueueItem in eventQueueItems)
            {
                try
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        cancel = true;

                        break;
                    }

                    await _eventBus.PublishAsync(eventQueueItem.IntegrationEvent);

                    await _integrationEventsDequeueService.MarkEventAsPublishedAsync(eventQueueItem);
                }
                catch (Exception ex)
                {
                    await _integrationEventsDequeueService.MarkEventAsNotPublishedAsync(eventQueueItem);
                }
            }

            if (cancel)
            {
                //https://stackoverflow.com/questions/36426937/what-is-the-difference-between-wait-vs-getawaiter-getresult
                //Mark all events with forward/dequeue batch id as NotPublished
                //In case of cancel the reenqueue needs to finish
                var requeuedItems =_integrationEventsDequeueService.RequeueEventsForBatchAsync(batchId).GetAwaiter().GetResult(); //In case of cancel the reenqueue needs to finish. GetAwaiter().GetResult() is used because it rearranges the stack trace in case of exception
            }
        }
    }
}
