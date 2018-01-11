using EnsureThat;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.BB.DDD.EntityFrameworkCore.Infrastructure.IntegrationEvents
{
    public class IntegrationEventsQueueItemConfiguration : IEntityTypeConfiguration<IntegrationEventsQueueItem>
    {
        private readonly string _schema;
        private readonly string _tableName;

        public IntegrationEventsQueueItemConfiguration(string schema = "dbo", string tableName = "IntegrationEventQueue")
        {
            _schema = EnsureArg.IsNotNullOrWhiteSpace(schema, nameof(schema));
            _tableName = EnsureArg.IsNotNullOrWhiteSpace(tableName, nameof(tableName));
        }

        public void Configure(EntityTypeBuilder<IntegrationEventsQueueItem> builder)
        {
            builder.ToTable(_tableName, _schema);

            builder.HasKey(e => e.EventId);

            builder.Property(e => e.EventId)
                .IsRequired();

            builder.Property(e => e.Content)
                .IsRequired();

            builder.Property(e => e.CreationTime)
                .IsRequired();

            builder.Property(e => e.State)
                .IsRequired();

            builder.Property(e => e.TimesSent)
                .IsRequired();

            builder.Property(e => e.EventTypeName)
                .IsRequired();
        }
    }
}
