using Axerrio.BB.DDD.Application.IntegrationEvents.Abstractions;
using EnsureThat;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.BB.DDD.Application.IntegrationEvents
{
    public class RabbitMQEventBus : IEventBus
    {
        private readonly ILogger<RabbitMQEventBus> _logger;

        public RabbitMQEventBus(ILogger<RabbitMQEventBus> logger)
        {
            logger = EnsureArg.IsNotNull(logger, nameof(logger));
        }

        public void Publish(IntegrationEvent @event)
        {
            _logger.LogInformation($"Event {@event.Id} created on {@event.CreationTimestamp.ToShortTimeString()} is published");
        }
    }
}
