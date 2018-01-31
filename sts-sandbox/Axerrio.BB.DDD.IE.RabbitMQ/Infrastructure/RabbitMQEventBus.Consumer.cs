using Axerrio.BB.DDD.Infrastructure.IntegrationEvents.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Axerrio.BB.DDD.IE.RabbitMQ.Infrastructure
{
    public partial class RabbitMQEventBus : IEventBusConsumer
    {
        private readonly IEventBusSubscriptionsService _eventBusSubscriptionsService;

        public Task StartConsuming(CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public Task StopConsuming(CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }
    }
}
