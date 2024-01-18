using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoHelper.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Updateforconversations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Conversations_VehicleLookups_VehicleLicensePlate",
                table: "Conversations");

            migrationBuilder.AddForeignKey(
                name: "FK_Conversations_VehicleLookups_VehicleLicensePlate",
                table: "Conversations",
                column: "VehicleLicensePlate",
                principalTable: "VehicleLookups",
                principalColumn: "LicensePlate",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Conversations_VehicleLookups_VehicleLicensePlate",
                table: "Conversations");

            migrationBuilder.AddForeignKey(
                name: "FK_Conversations_VehicleLookups_VehicleLicensePlate",
                table: "Conversations",
                column: "VehicleLicensePlate",
                principalTable: "VehicleLookups",
                principalColumn: "LicensePlate");
        }
    }
}
