using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Axerrio.BB.DDD.Application.IntegrationEvents.Abstractions
{
    public interface IntegrationEventsDequeueService
    {
        Task<IEnumerable<IntegrationEventsQueueItem>> DequeueEventsAsync(Guid batchId, CancellationToken cancellationToken = default(CancellationToken));
        Task MarkEventAsNotPublishedAsync(IntegrationEventsQueueItem eventQueueItem, CancellationToken cancellationToken = default(CancellationToken));
        Task MarkEventAsPublishedAsync(IntegrationEventsQueueItem eventQueueItem, CancellationToken cancellationToken = default(CancellationToken));
        //Task MarkEventAsPublishedFailedAsync(IntegrationEvent @event, CancellationToken cancellationToken = default(CancellationToken));

        Task<IEnumerable<IntegrationEventsQueueItem>> RequeueEventsForBatchAsync(Guid batchId, CancellationToken cancellationToken = default(CancellationToken));
    }
}
