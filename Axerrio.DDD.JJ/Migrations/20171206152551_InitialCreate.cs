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
                name: "MenuId",
                schema: "dbo",
                incrementBy: 5);

            migrationBuilder.CreateTable(
                name: "MenuStatus",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    MenuStatusId = table.Column<int>(nullable: true),
                    Name = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuStatus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MenuStatus_MenuStatus_MenuStatusId",
                        column: x => x.MenuStatusId,
                        principalSchema: "dbo",
                        principalTable: "MenuStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Menus",
                schema: "dbo",
                columns: table => new
                {
                    MenuId = table.Column<int>(nullable: false),
                    Description = table.Column<string>(maxLength: 255, nullable: false),
                    Id = table.Column<int>(nullable: false),
                    MenuStatusId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Menus", x => x.MenuId);
                    table.ForeignKey(
                        name: "FK_Menus_MenuStatus_Id",
                        column: x => x.Id,
                        principalSchema: "dbo",
                        principalTable: "MenuStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Menus_Id",
                schema: "dbo",
                table: "Menus",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_MenuStatus_MenuStatusId",
                schema: "dbo",
                table: "MenuStatus",
                column: "MenuStatusId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Menus",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "MenuStatus",
                schema: "dbo");

            migrationBuilder.DropSequence(
                name: "MenuId",
                schema: "dbo");
        }
    }
}
