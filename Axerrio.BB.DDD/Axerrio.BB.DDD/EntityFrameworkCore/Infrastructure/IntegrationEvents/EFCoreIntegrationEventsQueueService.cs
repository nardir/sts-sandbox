using Axerrio.BB.DDD.Application.IntegrationEvents;
using Axerrio.BB.DDD.Application.IntegrationEvents.Abstractions;
using Axerrio.BB.DDD.EntityFrameworkCore.Infrastructure.IntegrationEvents.Abstractions;
using EnsureThat;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Axerrio.BB.DDD.EntityFrameworkCore.Infrastructure.IntegrationEvents
{
    public class EFCoreIntegrationEventsQueueService<TContext> : IIntegrationEventsQueueService
        where TContext : DbContext, IIntegrationEventsDbContext
    {
        private readonly TContext _context;
        private readonly ILogger<EFCoreIntegrationEventsQueueService<TContext>> _logger;

        public EFCoreIntegrationEventsQueueService(TContext context, ILogger<EFCoreIntegrationEventsQueueService<TContext>> logger)
        {
            _context = EnsureArg.IsNotNull(context, nameof(context));
            _logger = EnsureArg.IsNotNull(logger, nameof(logger));

            //_context.Database.GetDbConnection().ConnectionString

        }

        public Task<IList<IntegrationEvent>> DequeueEventsAsync(int maxNumberOfEvents, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public void EnqueueEvent(IntegrationEvent @event)
        {
            EnsureArg.IsNotNull(@event, nameof(@event));

            _context.IntegrationEventsQueueItems.Add(new IntegrationEventsQueueItem(@event));
        }

        public Task MarkEventAsNotPublishedAsync(IntegrationEvent @event, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public Task MarkEventAsPublishedAsync(IntegrationEvent @event, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }
    }
}
