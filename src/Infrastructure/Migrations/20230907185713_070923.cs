using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoHelper.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class _070923 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GarageServices_Garages_GarageId",
                table: "GarageServices");

            migrationBuilder.DropIndex(
                name: "IX_GarageServices_GarageId",
                table: "GarageServices");

            migrationBuilder.AddColumn<string>(
                name: "AccountId",
                table: "GarageServices",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "GarageItemId",
                table: "GarageServices",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AccountId",
                table: "Garages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_GarageServices_GarageItemId",
                table: "GarageServices",
                column: "GarageItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_GarageServices_Garages_GarageItemId",
                table: "GarageServices",
                column: "GarageItemId",
                principalTable: "Garages",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GarageServices_Garages_GarageItemId",
                table: "GarageServices");

            migrationBuilder.DropIndex(
                name: "IX_GarageServices_GarageItemId",
                table: "GarageServices");

            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "GarageServices");

            migrationBuilder.DropColumn(
                name: "GarageItemId",
                table: "GarageServices");

            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "Garages");

            migrationBuilder.CreateIndex(
                name: "IX_GarageServices_GarageId",
                table: "GarageServices",
                column: "GarageId");

            migrationBuilder.AddForeignKey(
                name: "FK_GarageServices_Garages_GarageId",
                table: "GarageServices",
                column: "GarageId",
                principalTable: "Garages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
