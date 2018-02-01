using Axerrio.BB.DDD.EntityFrameworkCore.Infrastructure.IntegrationEvents;
using Axerrio.BB.DDD.EntityFrameworkCore.Infrastructure.IntegrationEvents.Abstractions;
using EnsureThat;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.BB.DDD.IE.API.Application
{
    public class OrderingDbContext : DbContext, IStoreAndForwardEventBusDbContext
    {
        private readonly StoreAndForwardEventBusDatabaseOptions _storeAndForwardEventBusDatabaseOptions;

        public DbSet<IntegrationEventsQueueItem> IntegrationEventsQueueItems { get; set; }

        public StoreAndForwardEventBusDatabaseOptions StoreAndForwardEventBusDatabaseOptions => _storeAndForwardEventBusDatabaseOptions;

        public OrderingDbContext(DbContextOptions<OrderingDbContext> options, IOptions<StoreAndForwardEventBusDatabaseOptions> storeAndForwardEventBusDatabaseOptionsAccessor)
            : base(options)
        {
            _storeAndForwardEventBusDatabaseOptions = EnsureArg.IsNotNull(storeAndForwardEventBusDatabaseOptionsAccessor, nameof(storeAndForwardEventBusDatabaseOptionsAccessor)).Value;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new IntegrationEventsQueueItemConfiguration(modelBuilder, _storeAndForwardEventBusDatabaseOptions));
        }
    }
}
