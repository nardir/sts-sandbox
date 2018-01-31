using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Axerrio.BB.DDD.Infrastructure.IntegrationEvents.Abstractions
{
    public interface IEventBusConsumer
    {
        Task StartConsumeAsync(CancellationToken cancellationToken = default(CancellationToken));
        Task StopConsumeAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}