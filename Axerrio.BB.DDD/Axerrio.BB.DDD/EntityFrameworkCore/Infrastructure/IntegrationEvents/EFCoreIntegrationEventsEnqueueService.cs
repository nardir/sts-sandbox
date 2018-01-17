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
            EnsureArg.IsNotNull(eventQueueItem, nameof(eventQueueItem));

            eventQueueItem.EnqueuedTimestamp = DateTime.UtcNow;

            await _context.IntegrationEventsQueueItems.AddAsync(eventQueueItem); //Async because we have a sequence

            _logger.LogDebug($"Integration event queue item {eventQueueItem.EventQueueItemId} enqueued on {eventQueueItem.EnqueuedTimestamp} for event {eventQueueItem.EventId} with type {eventQueueItem.EventTypeName} content: {eventQueueItem.EventContent}");
        }
    }
}
