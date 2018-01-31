using Axerrio.BB.DDD.EntityFrameworkCore.Infrastructure.IntegrationEvents;
using Axerrio.BB.DDD.EntityFrameworkCore.Infrastructure.IntegrationEvents.Abstractions;
using Axerrio.BB.DDD.Infrastructure.IntegrationEvents.Abstractions;
using EnsureThat;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Axerrio.BB.DDD.EntityFrameworkCore.Infrastructure
{
    public class EFCoreStoreAndForwardPublisherEventBus<TContext> : IEventBusPublisher
        where TContext : DbContext, IIntegrationEventsDbContext
    {
        private readonly TContext _context;
        private readonly ILogger<EFCoreStoreAndForwardPublisherEventBus<TContext>> _logger;

        public delegate EFCoreStoreAndForwardPublisherEventBus<TContext> Factory();

        public EFCoreStoreAndForwardPublisherEventBus(ILogger<EFCoreStoreAndForwardPublisherEventBus<TContext>> logger
            , TContext context)
        {
            _context = EnsureArg.IsNotNull(context, nameof(context));
            _logger = EnsureArg.IsNotNull(logger, nameof(logger));
        }

        public async Task PublishAsync(IntegrationEvent @event, CancellationToken cancellationToken = default(CancellationToken))
        {
            EnsureArg.IsNotNull(@event, nameof(@event));

            var eventQueueItem = new IntegrationEventsQueueItem(@event);

            _logger.LogDebug($"Enqueueing integration event, queue item: {eventQueueItem.EventQueueItemId} event: {eventQueueItem.EventId}");

            EnsureArg.IsNotNull(eventQueueItem, nameof(eventQueueItem));

            eventQueueItem.EnqueuedTimestamp = DateTime.UtcNow;

            await _context.IntegrationEventsQueueItems.AddAsync(eventQueueItem); //Async because we have a sql sequence

            _logger.LogDebug($"Enqueued integration event, queue item: {eventQueueItem.EventQueueItemId} event: {eventQueueItem.EventId}");

            _logger.LogTrace($"Enqueued integration event, queue item: {eventQueueItem.EventQueueItemId} event: {eventQueueItem.EventId} type: {eventQueueItem.EventTypeName} content: {eventQueueItem.EventContent}");
        }
    }
}
