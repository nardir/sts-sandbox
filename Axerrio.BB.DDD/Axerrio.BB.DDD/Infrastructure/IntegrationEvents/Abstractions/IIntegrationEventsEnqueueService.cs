using Axerrio.BB.DDD.Application.IntegrationEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Axerrio.BB.DDD.Infrastructure.IntegrationEvents.Abstractions
{
    public interface IIntegrationEventsEnqueueService
    {
        Task EnqueueAsync(IntegrationEvent @event, CancellationToken cancellationToken = default(CancellationToken));
    }
}
