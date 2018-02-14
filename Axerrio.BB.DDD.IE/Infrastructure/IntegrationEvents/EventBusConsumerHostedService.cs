using Axerrio.BB.DDD.Infrastructure.Hosting.HostedServices.Abstractions;
using Axerrio.BB.DDD.Infrastructure.IntegrationEvents.Abstractions;
using EnsureThat;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Axerrio.BB.DDD.Infrastructure.IntegrationEvents
{
    public class EventBusConsumerHostedService<TEventBusConsumer> : BackgroundService
        where TEventBusConsumer : IEventBusConsumer
    {
        private readonly ILogger<EventBusConsumerHostedService<TEventBusConsumer>> _logger;
        private readonly IEventBusConsumer _eventBusConsumer;

        public EventBusConsumerHostedService(ILogger<EventBusConsumerHostedService<TEventBusConsumer>> logger, TEventBusConsumer eventBusConsumer)
        {
            _logger = EnsureArg.IsNotNull(logger, nameof(logger));
            _eventBusConsumer = EnsureArg.IsNotNull(eventBusConsumer, nameof(eventBusConsumer));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.Register(() => _logger.LogDebug($"{nameof(EventBusConsumerHostedService<TEventBusConsumer>)} is stopping."));

            await _eventBusConsumer.StartConsumerAsync(stoppingToken);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await _eventBusConsumer.StopConsumerAsync(cancellationToken);

            await base.StopAsync(cancellationToken);
        }
    }
}
