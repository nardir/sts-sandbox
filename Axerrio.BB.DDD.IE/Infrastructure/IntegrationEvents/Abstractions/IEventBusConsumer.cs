using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Axerrio.BB.DDD.Infrastructure.IntegrationEvents.Abstractions
{
    public interface IEventBusConsumer
    {
        Task StartConsumerAsync(CancellationToken cancellationToken = default(CancellationToken));
        Task StopConsumerAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}