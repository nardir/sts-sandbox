using Axerrio.BB.DDD.Application.IntegrationEvents;
using Axerrio.BB.DDD.Application.IntegrationEvents.Abstractions;
using Axerrio.BB.DDD.EntityFrameworkCore.Infrastructure.IntegrationEvents.Abstractions;
using EnsureThat;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.BB.DDD.EntityFrameworkCore.Infrastructure.IntegrationEvents
{
    public class EFCoreIntegrationEventsEnqueueService<TContext> : IIntegrationEventsEnqueueService
        where TContext : DbContext, IIntegrationEventsDbContext
    {
        private readonly TContext _context;
        private readonly ILogger<EFCoreIntegrationEventsEnqueueService<TContext>> _logger;

        public EFCoreIntegrationEventsEnqueueService(TContext context, ILogger<EFCoreIntegrationEventsEnqueueService<TContext>> logger)
        {
            _context = EnsureArg.IsNotNull(context, nameof(context));
            _logger = EnsureArg.IsNotNull(logger, nameof(logger));
        }

        public async Task EnqueueEventAsync(IntegrationEventsQueueItem eventQueueItem)
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