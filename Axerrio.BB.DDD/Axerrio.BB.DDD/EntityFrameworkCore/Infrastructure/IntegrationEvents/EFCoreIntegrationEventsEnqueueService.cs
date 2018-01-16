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

        public Task EnqueueEventAsync(IntegrationEventsQueueItem eventQueueItem)
        {
            EnsureArg.IsNotNull(eventQueueItem, nameof(eventQueueItem));

            return _context.IntegrationEventsQueueItems.AddAsync(eventQueueItem); //Async because we have a sequence
        }
    }
}
