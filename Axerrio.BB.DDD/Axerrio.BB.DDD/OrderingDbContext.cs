using Axerrio.BB.DDD.EntityFrameworkCore.Infrastructure.IntegrationEvents;
using Axerrio.BB.DDD.EntityFrameworkCore.Infrastructure.IntegrationEvents.Abstractions;
using EnsureThat;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.BB.DDD
{
    public class OrderingDbContext : DbContext, IIntegrationEventsDbContext
    {
        private readonly IntegrationEventsDatabaseOptions _integrationEventsDatabaseOptions;

        public DbSet<IntegrationEventsQueueItem> IntegrationEventsQueueItems { get; set; }

        public IntegrationEventsDatabaseOptions IntegrationEventsDatabaseOptions => _integrationEventsDatabaseOptions;

        public OrderingDbContext(DbContextOptions<OrderingDbContext> options, IOptionsSnapshot<IntegrationEventsDatabaseOptions> integrationEventsDatabaseOptionsAccessor)
            : base(options)
        {
            _integrationEventsDatabaseOptions = EnsureArg.IsNotNull(integrationEventsDatabaseOptionsAccessor, nameof(integrationEventsDatabaseOptionsAccessor)).Value;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new IntegrationEventsQueueItemConfiguration(_integrationEventsDatabaseOptions.Schema, _integrationEventsDatabaseOptions.TableName));
        }
    }
}
