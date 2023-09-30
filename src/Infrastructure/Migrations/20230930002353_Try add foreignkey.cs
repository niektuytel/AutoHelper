using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoHelper.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Tryaddforeignkey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GarageEmployees_Garages_GarageItemId",
                table: "GarageEmployees");

            migrationBuilder.DropIndex(
                name: "IX_GarageEmployees_GarageItemId",
                table: "GarageEmployees");

            migrationBuilder.DropColumn(
                name: "GarageItemId",
                table: "GarageEmployees");

            migrationBuilder.CreateIndex(
                name: "IX_GarageEmployees_GarageId",
                table: "GarageEmployees",
                column: "GarageId");

            migrationBuilder.AddForeignKey(
                name: "FK_GarageEmployees_Garages_GarageId",
                table: "GarageEmployees",
                column: "GarageId",
                principalTable: "Garages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GarageEmployees_Garages_GarageId",
                table: "GarageEmployees");

            migrationBuilder.DropIndex(
                name: "IX_GarageEmployees_GarageId",
                table: "GarageEmployees");

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
    }
}
