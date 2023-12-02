using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoHelper.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Changeserviceitem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Type",
                table: "GarageServices",
                newName: "GeneralType");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "VehicleServiceLogs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "GarageServices",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Title",
                table: "VehicleServiceLogs");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "GarageServices");

            migrationBuilder.RenameColumn(
                name: "GeneralType",
                table: "GarageServices",
                newName: "Type");
        }
    }
}
