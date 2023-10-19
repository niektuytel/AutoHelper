using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoHelper.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class setforeignkeychange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VehicleServiceLogs_VehicleLookups_VehicleLookupId",
                table: "VehicleServiceLogs");

            migrationBuilder.AddForeignKey(
                name: "FK_VehicleServiceLogs_VehicleLookups_VehicleLookupId",
                table: "VehicleServiceLogs",
                column: "VehicleLookupId",
                principalTable: "VehicleLookups",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VehicleServiceLogs_VehicleLookups_VehicleLookupId",
                table: "VehicleServiceLogs");

            migrationBuilder.AddForeignKey(
                name: "FK_VehicleServiceLogs_VehicleLookups_VehicleLookupId",
                table: "VehicleServiceLogs",
                column: "VehicleLookupId",
                principalTable: "VehicleLookups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
