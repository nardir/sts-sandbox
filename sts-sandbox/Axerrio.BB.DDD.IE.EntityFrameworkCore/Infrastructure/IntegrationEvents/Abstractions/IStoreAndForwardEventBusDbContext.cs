using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Axerrio.BB.DDD.EntityFrameworkCore.Infrastructure.IntegrationEvents.Abstractions
{
    public interface IStoreAndForwardEventBusDbContext
    {
        StoreAndForwardEventBusDatabaseOptions StoreAndForwardEventBusDatabaseOptions { get; }
        DbSet<IntegrationEventsQueueItem> IntegrationEventsQueueItems { get; set; }
    }
}