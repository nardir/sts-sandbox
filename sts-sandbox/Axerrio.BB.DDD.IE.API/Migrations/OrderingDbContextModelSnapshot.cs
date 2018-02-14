﻿// <auto-generated />
using Axerrio.BB.DDD.EntityFrameworkCore.Infrastructure.IntegrationEvents;
using Axerrio.BB.DDD.IE.API.Application;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using System;

namespace Axerrio.BB.DDD.IE.API.Migrations
{
    [DbContext(typeof(OrderingDbContext))]
    partial class OrderingDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.1-rtm-125")
                .HasAnnotation("Relational:Sequence:integrationevents.EventQueueItemId", "'EventQueueItemId', 'integrationevents', '1', '100', '', '', 'Int32', 'False'")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Axerrio.BB.DDD.EntityFrameworkCore.Infrastructure.IntegrationEvents.IntegrationEventsQueueItem", b =>
                {
                    b.Property<int>("EventQueueItemId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:HiLoSequenceName", "EventQueueItemId")
                        .HasAnnotation("SqlServer:HiLoSequenceSchema", "integrationevents")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.SequenceHiLo);

                    b.Property<DateTime>("EnqueuedTimestamp");

                    b.Property<string>("EventContent")
                        .IsRequired();

                    b.Property<DateTime>("EventCreationTimestamp");

                    b.Property<Guid>("EventId");

                    b.Property<string>("EventName")
                        .IsRequired();

                    b.Property<string>("EventTypeName")
                        .IsRequired();

                    b.Property<DateTime?>("LatestDequeuedTimestamp");

                    b.Property<int>("PublishAttempts")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(0);

                    b.Property<Guid?>("PublishBatchId");

                    b.Property<DateTime?>("PublishedFailedTimestamp");

                    b.Property<DateTime?>("PublishedTimestamp");

                    b.Property<DateTime?>("RequeuedTimestamp");

                    b.Property<int>("State");

                    b.HasKey("EventQueueItemId");

                    b.HasAlternateKey("EventId");

                    b.ToTable("EventQueueItem","integrationevents");
                });
#pragma warning restore 612, 618
        }
    }
}
