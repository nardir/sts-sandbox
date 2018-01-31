using Axerrio.BB.DDD.EntityFrameworkCore.Infrastructure.IntegrationEvents.Abstractions;
using EnsureThat;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Axerrio.BB.DDD.EntityFrameworkCore.Infrastructure.IntegrationEvents
{
    //public class EFCoreStoreAndForwardEventBusFactory<TContext>
    //    where TContext : DbContext, IIntegrationEventsDbContext
    //{
    //    private readonly ILogger<EFCoreStoreAndForwardPublisherEventBus<TContext>> _logger;
    //    private readonly TContext _context;

    //    public EFCoreStoreAndForwardEventBusFactory(ILogger<EFCoreStoreAndForwardPublisherEventBus<TContext>> logger
    //        , TContext context)
    //    {
    //        _logger = EnsureArg.IsNotNull(logger, nameof(logger));
    //        _context = EnsureArg.IsNotNull(context, nameof(context));
    //    }

    //    public EFCoreStoreAndForwardPublisherEventBus<TContext> Create()
    //    {
    //        return new EFCoreStoreAndForwardPublisherEventBus<TContext>(_logger, _context);
    //    }
    //}
}
