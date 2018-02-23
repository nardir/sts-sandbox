using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Axerrio.BB.DDD.Infrastructure.IntegrationEvents.Abstractions
{
    public interface IEventBusForwarder
    {
        Task ForwardAsync(CancellationToken cancellationToken = default(CancellationToken));
        Task RequeueOrMarkAsPublishedFailedPendingEventsAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}