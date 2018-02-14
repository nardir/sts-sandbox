using Axerrio.BB.DDD.Application.IntegrationEvents.Abstractions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.BB.DDD
{
    public class OrderCreatedIntegrationEventHandler : IIntegrationEventHandler<OrderCreatedIntegrationEvent>
    {
        ILogger<OrderCreatedIntegrationEventHandler> _logger;

        public OrderCreatedIntegrationEventHandler(ILogger<OrderCreatedIntegrationEventHandler> logger)
        {
            _logger = logger;
        }

        public Task HandleAsync(OrderCreatedIntegrationEvent @event)
        {
            _logger.LogInformation($"OrderCreated {@event.OrderNumber}");

            return Task.CompletedTask;
        }
    }
}
