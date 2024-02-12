using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoHelper.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class BBringchangestoprd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "GarageServices");

            migrationBuilder.AddColumn<int>(
                name: "VehicleFuelType",
                table: "GarageServices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "VehicleFuelType",
                table: "GarageLookupServices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_VehicleTimelineItems_VehicleServiceLogId",
                table: "VehicleTimelineItems",
                column: "VehicleServiceLogId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_VehicleTimelineItems_VehicleServiceLogId",
                table: "VehicleTimelineItems");

            migrationBuilder.DropColumn(
                name: "VehicleFuelType",
                table: "GarageServices");

            migrationBuilder.DropColumn(
                name: "VehicleFuelType",
                table: "GarageLookupServices");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "GarageServices",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
