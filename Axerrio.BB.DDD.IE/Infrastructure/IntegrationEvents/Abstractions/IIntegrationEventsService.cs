using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Axerrio.BB.DDD.Infrastructure.IntegrationEvents.Abstractions
{
    public interface IIntegrationEventsService
    {
        Task PublishAsync(IntegrationEvent @event, CancellationToken cancellationToken = default(CancellationToken));
    }
}