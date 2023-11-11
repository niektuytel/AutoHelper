using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoHelper.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Addindexonit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VehicleTimelineItem_VehicleLookups_VehicleLookupItemLicensePlate",
                table: "VehicleTimelineItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VehicleTimelineItem",
                table: "VehicleTimelineItem");

            migrationBuilder.DropIndex(
                name: "IX_VehicleTimelineItem_VehicleLookupItemLicensePlate",
                table: "VehicleTimelineItem");

            migrationBuilder.DropColumn(
                name: "VehicleLookupItemLicensePlate",
                table: "VehicleTimelineItem");

            migrationBuilder.RenameTable(
                name: "VehicleTimelineItem",
                newName: "VehicleTimelineItems");

            migrationBuilder.AddColumn<string>(
                name: "VehicleLicensePlate",
                table: "VehicleTimelineItems",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VehicleTimelineItems",
                table: "VehicleTimelineItems",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleTimelineItems_VehicleLicensePlate",
                table: "VehicleTimelineItems",
                column: "VehicleLicensePlate");

            migrationBuilder.AddForeignKey(
                name: "FK_VehicleTimelineItems_VehicleLookups_VehicleLicensePlate",
                table: "VehicleTimelineItems",
                column: "VehicleLicensePlate",
                principalTable: "VehicleLookups",
                principalColumn: "LicensePlate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VehicleTimelineItems_VehicleLookups_VehicleLicensePlate",
                table: "VehicleTimelineItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VehicleTimelineItems",
                table: "VehicleTimelineItems");

            migrationBuilder.DropIndex(
                name: "IX_VehicleTimelineItems_VehicleLicensePlate",
                table: "VehicleTimelineItems");

            migrationBuilder.DropColumn(
                name: "VehicleLicensePlate",
                table: "VehicleTimelineItems");

            migrationBuilder.RenameTable(
                name: "VehicleTimelineItems",
                newName: "VehicleTimelineItem");

            migrationBuilder.AddColumn<string>(
                name: "VehicleLookupItemLicensePlate",
                table: "VehicleTimelineItem",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_VehicleTimelineItem",
                table: "VehicleTimelineItem",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleTimelineItem_VehicleLookupItemLicensePlate",
                table: "VehicleTimelineItem",
                column: "VehicleLookupItemLicensePlate");

            migrationBuilder.AddForeignKey(
                name: "FK_VehicleTimelineItem_VehicleLookups_VehicleLookupItemLicensePlate",
                table: "VehicleTimelineItem",
                column: "VehicleLookupItemLicensePlate",
                principalTable: "VehicleLookups",
                principalColumn: "LicensePlate");
        }
    }
}
