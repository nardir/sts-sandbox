using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.BB.DDD.Application.IntegrationEvents.Abstractions
{
    public interface IEventBus: IEventBusPublishOnly
    {
        //void Subscribe<TIntegrationEvent, TIntegrationEventHandler>()
        //    where TIntegrationEvent : IntegrationEvent
        //    where TIntegrationEventHandler : IIntegrationEventHandler<TIntegrationEvent>;

        //void Subscribe<TIntegrationEventHandler>(string eventName)
        //    where TIntegrationEventHandler : IDynamicIntegrationEventHandler;

        //void Unsubscribe<TIntegrationEventHandler>(string eventName)
        //    where TIntegrationEventHandler : IDynamicIntegrationEventHandler;

        //void Unsubscribe<TIntegrationEvent, TIntegrationEventHandler>()
        //    where TIntegrationEvent : IntegrationEvent
        //    where TIntegrationEventHandler : IIntegrationEventHandler<TIntegrationEvent>;
    }
}