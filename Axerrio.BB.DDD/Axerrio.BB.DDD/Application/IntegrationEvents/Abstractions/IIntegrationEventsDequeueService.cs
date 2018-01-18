using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Axerrio.BB.DDD.Application.IntegrationEvents.Abstractions
{
    public interface IIntegrationEventsDequeueService
    {
        Task<IEnumerable<IntegrationEventsQueueItem>> DequeueEventsAsync(Guid batchId, CancellationToken cancellationToken = default(CancellationToken));
        Task RequeueOrFailEventAsync(IntegrationEventsQueueItem eventQueueItem, CancellationToken cancellationToken = default(CancellationToken));
        Task PublishEventAsync(IntegrationEventsQueueItem eventQueueItem, CancellationToken cancellationToken = default(CancellationToken));
        //Task MarkEventAsPublishedFailedAsync(IntegrationEvent @event, CancellationToken cancellationToken = default(CancellationToken));

        Task<IEnumerable<IntegrationEventsQueueItem>> RequeueEventsForBatchAsync(Guid batchId, CancellationToken cancellationToken = default(CancellationToken));
    }
}
