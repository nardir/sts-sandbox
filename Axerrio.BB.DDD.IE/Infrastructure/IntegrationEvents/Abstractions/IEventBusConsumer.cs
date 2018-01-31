using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Axerrio.BB.DDD.Infrastructure.IntegrationEvents.Abstractions
{
    public interface IEventBusConsumer
    {
        Task StartConsuming(CancellationToken cancellationToken = default(CancellationToken));
        Task StopConsuming(CancellationToken cancellationToken = default(CancellationToken));
    }
}