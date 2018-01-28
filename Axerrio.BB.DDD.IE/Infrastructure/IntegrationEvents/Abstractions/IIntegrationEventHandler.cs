using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Axerrio.BB.DDD.Infrastructure.IntegrationEvents.Abstractions
{
    public interface IIntegrationEventHandler<TIntegrationEvent>
        where TIntegrationEvent : IntegrationEvent
    {
        Task HandleAsync(TIntegrationEvent @event);      
    }

    public interface IIntegrationEventHandler
    {
        Task HandleAsync(dynamic @event);
    }
}
