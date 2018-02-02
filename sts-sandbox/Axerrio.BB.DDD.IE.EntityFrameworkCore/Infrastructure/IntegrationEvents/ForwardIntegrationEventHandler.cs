using Axerrio.BB.DDD.Infrastructure.IntegrationEvents.Abstractions;
using EnsureThat;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Axerrio.BB.DDD.EntityFrameworkCore.Infrastructure.IntegrationEvents
{
    public class ForwardIntegrationEventHandler : IIntegrationEventHandler
    {
        private readonly IEventBusPublisher _eventBusPublisher;

        public ForwardIntegrationEventHandler(IEventBusPublisher eventBusPublisher)
        {
            _eventBusPublisher = EnsureArg.IsNotNull(eventBusPublisher, nameof(eventBusPublisher));
        }

        public Task HandleAsync(string eventName, dynamic @event, CancellationToken cancellationToken = default(CancellationToken))
        {
            EnsureArg.IsNotNull(@event, nameof(@event));

            var eventMessage = JsonConvert.SerializeObject(@event);

            return _eventBusPublisher.PublishAsync(eventName, eventMessage, cancellationToken);
        }
    }
}