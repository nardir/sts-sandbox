using Axerrio.BB.DDD.Infrastructure.IntegrationEvents.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Axerrio.BB.DDD.Application.IntegrationEvents.Abstractions
{
    public interface IEventBusPublishOnly
    {
        Task PublishAsync(IntegrationEvent @event, CancellationToken cancellationToken = default(CancellationToken));
    }
}
