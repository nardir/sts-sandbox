using Axerrio.BB.DDD.EntityFrameworkCore.Infrastructure.IntegrationEvents.Abstractions;
using EnsureThat;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Axerrio.BB.DDD.EntityFrameworkCore.Infrastructure.IntegrationEvents
{
    public class EFCoreStoreAndForwardEventBusFactory<TContext>
        where TContext : DbContext, IIntegrationEventsDbContext
    {
        private readonly ILogger<EFCoreStoreAndForwardEventBus<TContext>> _logger;
        private readonly TContext _context;

        public EFCoreStoreAndForwardEventBusFactory(ILogger<EFCoreStoreAndForwardEventBus<TContext>> logger
            , TContext context)
        {
            _logger = EnsureArg.IsNotNull(logger, nameof(logger));
            _context = EnsureArg.IsNotNull(context, nameof(context));
        }

        public EFCoreStoreAndForwardEventBus<TContext> Create()
        {
            return new EFCoreStoreAndForwardEventBus<TContext>(_logger, _context);
        }
    }
}
