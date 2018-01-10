using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Axerrio.BB.DDD.Application.IntegrationEvents.Abstractions
{
    public interface IIntegrationEventsQueueService
    {
        void EnqueueEvent(IntegrationEvent @event);
        Task<IList<IntegrationEvent>> DequeueEventsAsync(int maxNumberOfEvents, CancellationToken cancellationToken = default(CancellationToken));
        Task MarkEventAsNotPublishedAsync(IntegrationEvent @event, CancellationToken cancellationToken = default(CancellationToken));
        Task MarkEventAsPublishedAsync(IntegrationEvent @event, CancellationToken cancellationToken = default(CancellationToken));
        //Task MarkEventAsPublishedFailedAsync(IntegrationEvent @event, CancellationToken cancellationToken = default(CancellationToken));
    }
}
