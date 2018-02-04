using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Axerrio.BB.DDD.Infrastructure.IntegrationEvents.Abstractions
{
    public interface IEventBusPublisher
    {
        Task PublishAsync(IntegrationEvent @event, CancellationToken cancellationToken = default(CancellationToken));
        Task PublishAsync(string eventName, Guid eventId, string eventMessage, CancellationToken cancellationToken = default(CancellationToken));
    }
}
