using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace AutoHelper.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class cleanvehiclelookup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Location",
                table: "VehicleLookups");

            migrationBuilder.DropColumn(
                name: "ReporterEmailAddress",
                table: "VehicleLookups");

            migrationBuilder.DropColumn(
                name: "ReporterPhoneNumber",
                table: "VehicleLookups");

            migrationBuilder.DropColumn(
                name: "ReporterWhatsappNumber",
                table: "VehicleLookups");

            migrationBuilder.DropColumn(
                name: "HasPickupService",
                table: "GarageLookups");

            migrationBuilder.DropColumn(
                name: "HasReplacementTransportService",
                table: "GarageLookups");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Geometry>(
                name: "Location",
                table: "VehicleLookups",
                type: "geography",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReporterEmailAddress",
                table: "VehicleLookups",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReporterPhoneNumber",
                table: "VehicleLookups",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReporterWhatsappNumber",
                table: "VehicleLookups",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HasPickupService",
                table: "GarageLookups",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasReplacementTransportService",
                table: "GarageLookups",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
