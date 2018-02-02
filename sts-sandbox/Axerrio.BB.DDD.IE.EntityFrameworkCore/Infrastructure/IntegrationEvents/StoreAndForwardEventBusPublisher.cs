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
    public class StoreAndForwardEventBusPublisher<TContext> : IEventBusPublisher
        where TContext : DbContext, IStoreAndForwardEventBusDbContext
    {
        private readonly TContext _context;
        private readonly ILogger<StoreAndForwardEventBusPublisher<TContext>> _logger;

        public delegate StoreAndForwardEventBusPublisher<TContext> Factory();

        public StoreAndForwardEventBusPublisher(ILogger<StoreAndForwardEventBusPublisher<TContext>> logger
            , TContext context)
        {
            _context = EnsureArg.IsNotNull(context, nameof(context));
            _logger = EnsureArg.IsNotNull(logger, nameof(logger));
        }

        public Task PublishAsync(IntegrationEvent @event, CancellationToken cancellationToken = default(CancellationToken))
        {
            EnsureArg.IsNotNull(@event, nameof(@event));

            //var eventQueueItem = new IntegrationEventsQueueItem(@event);
            var eventQueueItem = IntegrationEventsQueueItem.Create(@event);

            return PublishAsync(eventQueueItem, cancellationToken);
        }

        public Task PublishAsync(string eventName, string eventMessage, CancellationToken cancellationToken = default(CancellationToken))
        {
            EnsureArg.IsNotNullOrWhiteSpace(eventName, nameof(eventName));
            EnsureArg.IsNotNullOrWhiteSpace(eventMessage, nameof(eventMessage));

            var eventQueueItem = IntegrationEventsQueueItem.Create(eventName, eventMessage);

            return PublishAsync(eventQueueItem, cancellationToken);
        }

        private async Task PublishAsync(IntegrationEventsQueueItem eventQueueItem, CancellationToken cancellationToken = default(CancellationToken))
        {
            _logger.LogDebug($"Enqueueing integration event, queue item: {eventQueueItem.EventQueueItemId} event: {eventQueueItem.EventId}");

            EnsureArg.IsNotNull(eventQueueItem, nameof(eventQueueItem));

            eventQueueItem.EnqueuedTimestamp = DateTime.UtcNow;

            await _context.IntegrationEventsQueueItems.AddAsync(eventQueueItem); //Async because we have a sql sequence

            _logger.LogDebug($"Enqueued integration event, queue item: {eventQueueItem.EventQueueItemId} event: {eventQueueItem.EventId}");

            _logger.LogTrace($"Enqueued integration event, queue item: {eventQueueItem.EventQueueItemId} event: {eventQueueItem.EventId} type: {eventQueueItem.EventTypeName} content: {eventQueueItem.EventContent}");
        }
    }
}
