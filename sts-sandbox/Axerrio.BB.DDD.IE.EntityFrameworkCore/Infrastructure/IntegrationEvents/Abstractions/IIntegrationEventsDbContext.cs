using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Axerrio.BB.DDD.EntityFrameworkCore.Infrastructure.IntegrationEvents.Abstractions
{
    public interface IIntegrationEventsDbContext
    {
        EFCoreIntegrationEventsDatabaseOptions IntegrationEventsDatabaseOptions { get; }
        DbSet<IntegrationEventsQueueItem> IntegrationEventsQueueItems { get; set; }
    }
}
