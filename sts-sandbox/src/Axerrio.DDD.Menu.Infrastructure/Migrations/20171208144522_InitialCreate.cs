using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Axerrio.DDD.Menu.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");            

            migrationBuilder.CreateSequence<int>(
                name: "ArtistId",
                schema: "dbo",
                incrementBy: 5);

            migrationBuilder.CreateSequence<int>(
                name: "MenuId",
                schema: "dbo",
                incrementBy: 5);

            migrationBuilder.CreateTable(
                name: "MenuStatus",
                schema: "dbo",
                columns: table => new
                {
                    MenuStatusId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuStatus", x => x.MenuStatusId);
                });

            migrationBuilder.CreateTable(
                name: "Menus",
                schema: "dbo",
                columns: table => new
                {
                    MenuId = table.Column<int>(nullable: false),
                    Description = table.Column<string>(maxLength: 255, nullable: false),
                    MenuStatusId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Menus", x => x.MenuId);
                    table.ForeignKey(
                        name: "FK_Menus_MenuStatus_MenuStatusId",
                        column: x => x.MenuStatusId,
                        principalSchema: "dbo",
                        principalTable: "MenuStatus",
                        principalColumn: "MenuStatusId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Artist",
                schema: "dbo",
                columns: table => new
                {
                    ArtistId = table.Column<int>(nullable: false),
                    EmailAddress = table.Column<string>(maxLength: 255, nullable: false),
                    FirstName = table.Column<string>(maxLength: 255, nullable: false),
                    LastName = table.Column<string>(maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Artist", x => x.ArtistId);
                    table.ForeignKey(
                        name: "FK_Artist_Menus_ArtistId",
                        column: x => x.ArtistId,
                        principalSchema: "dbo",
                        principalTable: "Menus",
                        principalColumn: "MenuId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RequestInfo",
                schema: "dbo",
                columns: table => new
                {
                    MenuIdentity = table.Column<int>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    Requester = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestInfo", x => x.MenuIdentity);
                    table.ForeignKey(
                        name: "FK_RequestInfo_Menus_MenuIdentity",
                        column: x => x.MenuIdentity,
                        principalSchema: "dbo",
                        principalTable: "Menus",
                        principalColumn: "MenuId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Menus_MenuStatusId",
                schema: "dbo",
                table: "Menus",
                column: "MenuStatusId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Artist",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "RequestInfo",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Menus",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "MenuStatus",
                schema: "dbo");

            migrationBuilder.DropSequence(
                name: "Artist",
                schema: "dbo");

            migrationBuilder.DropSequence(
                name: "ArtistId",
                schema: "dbo");

            migrationBuilder.DropSequence(
                name: "MenuId",
                schema: "dbo");
        }
    }
}
