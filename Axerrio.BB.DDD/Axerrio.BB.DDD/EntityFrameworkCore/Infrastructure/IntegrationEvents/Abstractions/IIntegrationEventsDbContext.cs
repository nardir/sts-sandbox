using Axerrio.BB.DDD.Application.IntegrationEvents;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.BB.DDD.EntityFrameworkCore.Infrastructure.IntegrationEvents.Abstractions
{
    public interface IIntegrationEventsDbContext
    {
        IntegrationEventsDatabaseOptions IntegrationEventsDatabaseOptions { get; }

        DbSet<IntegrationEventsQueueItem> IntegrationEventsQueueItems { get; set; }
    }
}