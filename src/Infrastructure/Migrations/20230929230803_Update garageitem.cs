using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoHelper.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Updategarageitem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContactItem_Garages_GarageItemId",
                table: "ContactItem");

            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_Garages_GarageItemId",
                table: "Vehicles");

            migrationBuilder.DropIndex(
                name: "IX_Vehicles_GarageItemId",
                table: "Vehicles");

            migrationBuilder.DropIndex(
                name: "IX_ContactItem_GarageItemId",
                table: "ContactItem");

            migrationBuilder.DropColumn(
                name: "GarageItemId",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "GarageItemId",
                table: "ContactItem");

            migrationBuilder.AddColumn<Guid>(
                name: "GarageId",
                table: "GarageEmployeeWorkExperienceItems",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "GarageItemId",
                table: "GarageEmployees",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_GarageEmployees_GarageItemId",
                table: "GarageEmployees",
                column: "GarageItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_GarageEmployees_Garages_GarageItemId",
                table: "GarageEmployees",
                column: "GarageItemId",
                principalTable: "Garages",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GarageEmployees_Garages_GarageItemId",
                table: "GarageEmployees");

            migrationBuilder.DropIndex(
                name: "IX_GarageEmployees_GarageItemId",
                table: "GarageEmployees");

            migrationBuilder.DropColumn(
                name: "GarageId",
                table: "GarageEmployeeWorkExperienceItems");

            migrationBuilder.DropColumn(
                name: "GarageItemId",
                table: "GarageEmployees");

            migrationBuilder.AddColumn<Guid>(
                name: "GarageItemId",
                table: "Vehicles",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "GarageItemId",
                table: "ContactItem",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_GarageItemId",
                table: "Vehicles",
                column: "GarageItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ContactItem_GarageItemId",
                table: "ContactItem",
                column: "GarageItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_ContactItem_Garages_GarageItemId",
                table: "ContactItem",
                column: "GarageItemId",
                principalTable: "Garages",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_Garages_GarageItemId",
                table: "Vehicles",
                column: "GarageItemId",
                principalTable: "Garages",
                principalColumn: "Id");
        }
    }
}
