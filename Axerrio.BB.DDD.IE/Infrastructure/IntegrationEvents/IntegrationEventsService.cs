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
    public class IntegrationEventsService<TEventBusPublisher> : IIntegrationEventsService
        where TEventBusPublisher: IEventBusPublisher
    {
        private readonly IEventBusPublisher _eventBus;

        public IntegrationEventsService(TEventBusPublisher eventBus)
        {
            _eventBus = EnsureArg.IsNotNull(eventBus, nameof(eventBus));
        }

        public Task PublishAsync(IntegrationEvent @event, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _eventBus.PublishAsync(@event, cancellationToken);
        }
    }
}
