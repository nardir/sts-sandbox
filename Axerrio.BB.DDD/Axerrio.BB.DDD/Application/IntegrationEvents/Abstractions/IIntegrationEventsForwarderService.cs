using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Axerrio.BB.DDD.Application.IntegrationEvents.Abstractions
{
    public interface IIntegrationEventsForwarderService
    {
        Task ForwardAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}
