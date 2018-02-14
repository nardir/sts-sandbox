using Axerrio.BB.DDD.Application.IntegrationEvents.Abstractions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.BB.DDD
{
    public class OrderCreatedIntegrationEventHandlerDynamic : IDynamicIntegrationEventHandler
    {
        ILogger<OrderCreatedIntegrationEventHandlerDynamic> _logger;

        public OrderCreatedIntegrationEventHandlerDynamic(ILogger<OrderCreatedIntegrationEventHandlerDynamic> logger)
        {
            _logger = logger;
        }

        public Task HandleAsync(dynamic @event)
        {
            _logger.LogInformation($"{@event.OrderNumber} {@event.CustomerCode}");

            return Task.CompletedTask;
        }
    }
}
