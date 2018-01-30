using Axerrio.BB.DDD.EntityFrameworkCore.Infrastructure.IntegrationEvents.Abstractions;
using Axerrio.BB.DDD.Infrastructure.IntegrationEvents.Abstractions;
using EnsureThat;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Axerrio.BB.DDD.EntityFrameworkCore.Infrastructure
{
    public class EFCoreStoreAndForwardEventBus<TContext> : IEventBusPublisher
        where TContext : DbContext, IIntegrationEventsDbContext
    {
        private readonly TContext _context;
        private readonly ILogger<EFCoreStoreAndForwardEventBus<TContext>> _logger;

        public delegate EFCoreStoreAndForwardEventBus<TContext> Factory();

        public EFCoreStoreAndForwardEventBus(ILogger<EFCoreStoreAndForwardEventBus<TContext>> logger
            , TContext context)
        {
            _context = EnsureArg.IsNotNull(context, nameof(context));
            _logger = EnsureArg.IsNotNull(logger, nameof(logger));
        }

        public Task PublishAsync(IntegrationEvent @event, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }
    }
}
