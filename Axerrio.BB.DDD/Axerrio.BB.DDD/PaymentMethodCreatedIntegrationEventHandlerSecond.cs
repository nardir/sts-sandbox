using Axerrio.BB.DDD.Application.IntegrationEvents.Abstractions;
using EnsureThat;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.BB.DDD
{
    public class PaymentMethodCreatedIntegrationEventHandlerSecond : IIntegrationEventHandler<PaymentMethodCreatedIntegrationEvent>
    {
        private readonly ILogger<PaymentMethodCreatedIntegrationEventHandlerSecond> _logger;

        public PaymentMethodCreatedIntegrationEventHandlerSecond(ILogger<PaymentMethodCreatedIntegrationEventHandlerSecond> logger)
        {
            _logger = EnsureArg.IsNotNull(logger, nameof(logger));
        }

        public Task HandleAsync(PaymentMethodCreatedIntegrationEvent @event)
        {
            _logger.LogInformation($"Second Handler: {@event.PaymentMethod.CardTypeId}");

            return Task.CompletedTask;
        }
    }
}
