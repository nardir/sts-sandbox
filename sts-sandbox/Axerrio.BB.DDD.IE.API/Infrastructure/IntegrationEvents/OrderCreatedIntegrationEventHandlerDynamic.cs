using Axerrio.BB.DDD.Domain.IntegrationEvents.Abstractions;
using Axerrio.BB.DDD.Infrastructure.IntegrationEvents.Abstractions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Axerrio.BB.DDD.IE.API.Infrastructure.IntegrationEvents
{
    public class OrderCreatedIntegrationEventHandlerDynamic : IIntegrationEventHandler
    {
        ILogger<OrderCreatedIntegrationEventHandlerDynamic> _logger;

        public OrderCreatedIntegrationEventHandlerDynamic(ILogger<OrderCreatedIntegrationEventHandlerDynamic> logger)
        {
            _logger = logger;
        }

        public Task HandleAsync(string eventName, dynamic @event, CancellationToken cancellationToken = default(CancellationToken))
        {
            _logger.LogInformation($"{@event.OrderNumber} {@event.CustomerCode}");

            return Task.CompletedTask;
        }
    }
}
