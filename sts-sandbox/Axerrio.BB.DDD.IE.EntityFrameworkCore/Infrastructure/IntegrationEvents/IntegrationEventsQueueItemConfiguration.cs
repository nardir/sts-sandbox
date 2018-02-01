using EnsureThat;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Axerrio.BB.DDD.EntityFrameworkCore.Infrastructure.IntegrationEvents
{
    public class IntegrationEventsQueueItemConfiguration : IEntityTypeConfiguration<IntegrationEventsQueueItem>
    {
        private readonly string _schema;
        private readonly string _tableName;

        private readonly string _sequenceName = "EventQueueItemId";
        private readonly int _sequenceIncrement = 100;

        private readonly ModelBuilder _modelBuilder;

        public IntegrationEventsQueueItemConfiguration(ModelBuilder modelBuilder, StoreAndForwardEventBusDatabaseOptions integrationEventsDatabaseOptions)
        {
            _modelBuilder = EnsureArg.IsNotNull(modelBuilder, nameof(modelBuilder));

            EnsureArg.IsNotNull(integrationEventsDatabaseOptions, nameof(integrationEventsDatabaseOptions));

            _schema = EnsureArg.IsNotNullOrWhiteSpace(integrationEventsDatabaseOptions.Schema, nameof(integrationEventsDatabaseOptions.Schema));
            _tableName = EnsureArg.IsNotNullOrWhiteSpace(integrationEventsDatabaseOptions.TableName, nameof(integrationEventsDatabaseOptions.TableName));
        }

        public void Configure(EntityTypeBuilder<IntegrationEventsQueueItem> entityTypeBuilder)
        {
            _modelBuilder.HasSequence<int>(_sequenceName, _schema)
                .IncrementsBy(_sequenceIncrement);

            entityTypeBuilder.ToTable(_tableName, _schema);

            entityTypeBuilder.Ignore(eqi => eqi.IntegrationEvent);

            //Queue properties
            //PK
            entityTypeBuilder.Property(eqi => eqi.EventQueueItemId)
                .IsRequired()
                .ForSqlServerUseSequenceHiLo(_sequenceName, _schema);

            entityTypeBuilder.HasKey(eqi => eqi.EventQueueItemId);

            entityTypeBuilder.Property(eqi => eqi.State)
                .IsRequired();

            entityTypeBuilder.Property(eqi => eqi.PublishAttempts)
                .IsRequired()
                .HasDefaultValue(0);

            entityTypeBuilder.Property(eqi => eqi.PublishBatchId)
                .IsRequired(false);

            entityTypeBuilder.Property(eqi => eqi.EnqueuedTimestamp)
                .IsRequired();
            //.HasDefaultValue(DateTime.UtcNow);
            //.HasDefaultValueSql("getutcdate()");

            entityTypeBuilder.Property(eqi => eqi.LatestDequeuedTimestamp)
                .IsRequired(false);

            entityTypeBuilder.Property(eqi => eqi.PublishedTimestamp)
                .IsRequired(false);

            entityTypeBuilder.Property(eqi => eqi.PublishedFailedTimestamp)
                .IsRequired(false);

            entityTypeBuilder.Property(eqi => eqi.RequeuedTimestamp)
                .IsRequired(false);


            //Event properties
            entityTypeBuilder.Property(eqi => eqi.EventId)
                .IsRequired();

            entityTypeBuilder.HasAlternateKey(eqi => eqi.EventId);

            entityTypeBuilder.Property(eqi => eqi.EventCreationTimestamp)
                .IsRequired();

            entityTypeBuilder.Property(e => e.EventContent)
                .IsRequired();

            entityTypeBuilder.Property(e => e.EventTypeName)
                .IsRequired();
        }
    }
}
