using Axerrio.BB.DDD.Infrastructure.IntegrationEvents.Abstractions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Axerrio.BB.DDD.IE.API.Infrastructure.IntegrationEvents
{
    public class OrderCreatedIntegrationEventHandler : IIntegrationEventHandler<OrderCreatedIntegrationEvent>
    {
        ILogger<OrderCreatedIntegrationEventHandler> _logger;

        public OrderCreatedIntegrationEventHandler(ILogger<OrderCreatedIntegrationEventHandler> logger)
        {
            _logger = logger;
        }

        public Task HandleAsync(OrderCreatedIntegrationEvent @event, CancellationToken cancellationToken = default(CancellationToken))
        {
            _logger.LogInformation($"OrderCreated {@event.OrderNumber}");

            return Task.CompletedTask;
        }
    }
}
