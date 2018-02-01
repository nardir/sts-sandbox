using Axerrio.BB.DDD.Infrastructure.IntegrationEvents.Abstractions;
using Axerrio.BB.DDD.Infrastructure.Scheduling.Abstractions;
using EnsureThat;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Axerrio.BB.DDD.EntityFrameworkCore.Infrastructure.IntegrationEvents
{
    public class StoreAndForwardEventBusConsumerJob : TimedJob
    {
        private readonly IEventBusForwarder _eventBusForwarder;

        public StoreAndForwardEventBusConsumerJob(IEventBusForwarder eventBusForwarder, ILogger<StoreAndForwardEventBusConsumerJob> logger)
            : base(logger)
        {
            _eventBusForwarder = EnsureArg.IsNotNull(eventBusForwarder, nameof(eventBusForwarder));
        }

        public override Task ExecuteAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return _eventBusForwarder.ForwardAsync(cancellationToken);
        }
    }
}
