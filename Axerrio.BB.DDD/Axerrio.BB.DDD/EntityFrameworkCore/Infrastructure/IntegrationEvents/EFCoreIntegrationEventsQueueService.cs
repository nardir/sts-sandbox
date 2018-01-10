using Axerrio.BB.DDD.Application.IntegrationEvents;
using Axerrio.BB.DDD.Application.IntegrationEvents.Abstractions;
using Axerrio.BB.DDD.EntityFrameworkCore.Infrastructure.IntegrationEvents.Abstractions;
using Microsoft.EntityFrameworkCore;
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
        public Task<IList<IntegrationEvent>> DequeueEventsAsync(int maxNumberOfEvents, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public void EnqueueEvent(IntegrationEvent @event)
        {
            throw new NotImplementedException();
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
