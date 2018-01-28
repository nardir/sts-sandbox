using Axerrio.BB.DDD.Application.IntegrationEvents.Abstractions;
using Axerrio.BB.DDD.Infrastructure.IntegrationEvents.Abstractions;
using EnsureThat;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Axerrio.BB.DDD.Application.IntegrationEvents
{
    public class StoreAndForwardIntegrationEventsService<TEventBus> : IIntegrationEventsService
        where TEventBus : IEventBusPublishOnly
    {
        private readonly ILogger<StoreAndForwardIntegrationEventsService<TEventBus>> _logger;
        private readonly IEventBusPublishOnly _eventBus;

        public StoreAndForwardIntegrationEventsService(IEventBusPublishOnlyFactory eventBusPublishOnlyFactory, ILogger<StoreAndForwardIntegrationEventsService<TEventBus>> logger)
        {
            var factory = EnsureArg.IsNotNull(eventBusPublishOnlyFactory, nameof(eventBusPublishOnlyFactory));

            _logger = EnsureArg.IsNotNull(logger, nameof(logger));

            _eventBus = factory.Create<TEventBus>();
        }

        public async Task PublishAsync(IntegrationEvent @event, CancellationToken cancellationToken = default(CancellationToken))
        {
            await _eventBus.PublishAsync(@event, cancellationToken);
        }
    }
}
