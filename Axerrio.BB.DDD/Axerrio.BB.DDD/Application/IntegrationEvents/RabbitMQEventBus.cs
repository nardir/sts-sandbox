using Axerrio.BB.DDD.Application.IntegrationEvents.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.BB.DDD.Application.IntegrationEvents
{
    public class RabbitMQEventBus : IEventBus
    {
        public void Publish(IntegrationEvent @event)
        {
            throw new NotImplementedException();
        }
    }
}
