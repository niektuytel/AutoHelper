using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoHelper.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MakeServicelogmorespecificanddynamic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GarageLookups_GarageLookupLargeItem_LargeDataId",
                table: "GarageLookups");

            migrationBuilder.DropTable(
                name: "GarageLookupLargeItem");

            migrationBuilder.DropIndex(
                name: "IX_GarageLookups_LargeDataId",
                table: "GarageLookups");

            migrationBuilder.DropColumn(
                name: "LargeDataId",
                table: "GarageLookups");

            migrationBuilder.RenameColumn(
                name: "GeneralType",
                table: "GarageServices",
                newName: "VehicleType");

            migrationBuilder.AddColumn<Guid>(
                name: "GarageServiceId",
                table: "VehicleServiceLogs",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ExpectedNextDateIsRequired",
                table: "GarageServices",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ExpectedNextOdometerReadingIsRequired",
                table: "GarageServices",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "GarageServices",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GarageServiceId",
                table: "VehicleServiceLogs");

            migrationBuilder.DropColumn(
                name: "ExpectedNextDateIsRequired",
                table: "GarageServices");

            migrationBuilder.DropColumn(
                name: "ExpectedNextOdometerReadingIsRequired",
                table: "GarageServices");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "GarageServices");

            migrationBuilder.RenameColumn(
                name: "VehicleType",
                table: "GarageServices",
                newName: "GeneralType");

            migrationBuilder.AddColumn<Guid>(
                name: "LargeDataId",
                table: "GarageLookups",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "GarageLookupLargeItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GoogleApiDetailsJson = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GarageLookupLargeItem", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GarageLookups_LargeDataId",
                table: "GarageLookups",
                column: "LargeDataId");

            migrationBuilder.AddForeignKey(
                name: "FK_GarageLookups_GarageLookupLargeItem_LargeDataId",
                table: "GarageLookups",
                column: "LargeDataId",
                principalTable: "GarageLookupLargeItem",
                principalColumn: "Id");
        }
    }
}
