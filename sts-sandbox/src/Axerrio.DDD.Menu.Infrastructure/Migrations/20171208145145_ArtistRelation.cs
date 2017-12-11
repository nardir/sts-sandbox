using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Axerrio.DDD.Menu.Migrations
{
    public partial class ArtistRelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Artist_Menus_ArtistId",
                schema: "dbo",
                table: "Artist");

            migrationBuilder.AddColumn<int>(
                name: "ArtistId",
                schema: "dbo",
                table: "Menus",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Menus_ArtistId",
                schema: "dbo",
                table: "Menus",
                column: "ArtistId");

            migrationBuilder.AddForeignKey(
                name: "FK_Menus_Artist_ArtistId",
                schema: "dbo",
                table: "Menus",
                column: "ArtistId",
                principalSchema: "dbo",
                principalTable: "Artist",
                principalColumn: "ArtistId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Menus_Artist_ArtistId",
                schema: "dbo",
                table: "Menus");

            migrationBuilder.DropIndex(
                name: "IX_Menus_ArtistId",
                schema: "dbo",
                table: "Menus");

            migrationBuilder.DropColumn(
                name: "ArtistId",
                schema: "dbo",
                table: "Menus");

            migrationBuilder.AddForeignKey(
                name: "FK_Artist_Menus_ArtistId",
                schema: "dbo",
                table: "Artist",
                column: "ArtistId",
                principalSchema: "dbo",
                principalTable: "Menus",
                principalColumn: "MenuId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
