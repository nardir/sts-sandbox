using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Axerrio.BB.DDD.Infrastructure.IntegrationEvents.Abstractions
{
    public interface IIntegrationEventHandler<TIntegrationEvent>
        where TIntegrationEvent : IntegrationEvent
    {
        Task HandleAsync(string eventName, TIntegrationEvent @event, CancellationToken cancellationToken = default(CancellationToken));      
    }

    public interface IIntegrationEventHandler
    {
        Task HandleAsync(string eventName, dynamic @event, CancellationToken cancellationToken = default(CancellationToken));
    }
}