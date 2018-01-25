using Axerrio.BB.DDD.Application.IntegrationEvents.Abstractions;
using EnsureThat;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.BB.DDD
{
    public class PaymentMethodCreatedIntegrationEventHandler : IIntegrationEventHandler<PaymentMethodCreatedIntegrationEvent>
    {
        private readonly ILogger<PaymentMethodCreatedIntegrationEventHandler> _logger;

        public PaymentMethodCreatedIntegrationEventHandler(ILogger<PaymentMethodCreatedIntegrationEventHandler> logger)
        {
            _logger = EnsureArg.IsNotNull(logger, nameof(logger));
        }

        public Task HandleAsync(PaymentMethodCreatedIntegrationEvent @event)
        {
            _logger.LogInformation($"{nameof(PaymentMethodCreatedIntegrationEvent)}, {@event.CreationTimestamp} {@event.Id} {@event.PaymentMethod.CardTypeId} {@event.PaymentMethod.CardHolderName}");

            //await Task.Delay(TimeSpan.FromSeconds(3));

            return Task.CompletedTask;
        }
    }
}
