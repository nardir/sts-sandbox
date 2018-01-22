using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Axerrio.BB.DDD.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "integrationevents");

            migrationBuilder.CreateSequence<int>(
                name: "EventQueueItemId",
                schema: "integrationevents",
                incrementBy: 100);

            migrationBuilder.CreateTable(
                name: "EventQueueItem",
                schema: "integrationevents",
                columns: table => new
                {
                    EventQueueItemId = table.Column<int>(nullable: false),
                    EnqueuedTimestamp = table.Column<DateTime>(nullable: false),
                    EventContent = table.Column<string>(nullable: false),
                    EventCreationTimestamp = table.Column<DateTime>(nullable: false),
                    EventId = table.Column<Guid>(nullable: false),
                    EventTypeName = table.Column<string>(nullable: false),
                    LatestDequeuedTimestamp = table.Column<DateTime>(nullable: true),
                    PublishAttempts = table.Column<int>(nullable: false, defaultValue: 0),
                    PublishBatchId = table.Column<Guid>(nullable: true),
                    PublishedFailedTimestamp = table.Column<DateTime>(nullable: true),
                    PublishedTimestamp = table.Column<DateTime>(nullable: true),
                    RequeuedTimestamp = table.Column<DateTime>(nullable: true),
                    State = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventQueueItem", x => x.EventQueueItemId);
                    table.UniqueConstraint("AK_EventQueueItem_EventId", x => x.EventId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventQueueItem",
                schema: "integrationevents");

            migrationBuilder.DropSequence(
                name: "EventQueueItemId",
                schema: "integrationevents");
        }
    }
}
