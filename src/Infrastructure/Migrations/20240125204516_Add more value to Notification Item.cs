using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoHelper.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddmorevaluetoNotificationItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Type",
                table: "Notifications",
                newName: "VehicleType");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfBirth",
                table: "VehicleLookups",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GeneralType",
                table: "Notifications",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "TriggerDate",
                table: "Notifications",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateOfBirth",
                table: "VehicleLookups");

            migrationBuilder.DropColumn(
                name: "GeneralType",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "TriggerDate",
                table: "Notifications");

            migrationBuilder.RenameColumn(
                name: "VehicleType",
                table: "Notifications",
                newName: "Type");
        }
    }
}
