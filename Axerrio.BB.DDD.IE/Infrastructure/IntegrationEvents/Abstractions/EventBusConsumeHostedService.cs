using Axerrio.BB.DDD.Infrastructure.Hosting.HostedServices.Abstractions;
using EnsureThat;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Axerrio.BB.DDD.Infrastructure.IntegrationEvents.Abstractions
{
    public class EventBusConsumeHostedService : BackgroundService
    {
        private readonly ILogger<EventBusConsumeHostedService> _logger;
        private readonly IEventBusConsumer _eventBusConsumer;

        public EventBusConsumeHostedService(ILogger<EventBusConsumeHostedService> logger, IEventBusConsumer eventBusConsumer)
        {
            _logger = EnsureArg.IsNotNull(logger, nameof(logger));
            _eventBusConsumer = EnsureArg.IsNotNull(eventBusConsumer, nameof(eventBusConsumer));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.Register(() => _logger.LogDebug($"{nameof(EventBusConsumeHostedService)} is stopping."));

            await _eventBusConsumer.StartConsumeAsync(stoppingToken);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await _eventBusConsumer.StopConsumeAsync(cancellationToken);

            await base.StopAsync(cancellationToken);
        }
    }
}
