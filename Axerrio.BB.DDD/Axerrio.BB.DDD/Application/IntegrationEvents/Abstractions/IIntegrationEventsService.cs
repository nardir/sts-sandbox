using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Axerrio.BB.DDD.Application.IntegrationEvents.Abstractions
{
    public interface IIntegrationEventsService
    {
        //Task PublishAsync(IntegrationEvent @event, CancellationToken cancellationToken = default(CancellationToken));
        void Publish(IntegrationEvent @event);
    }
}