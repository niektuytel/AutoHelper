using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoHelper.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Editnaming : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MOTExpiryDate",
                table: "VehicleLookups",
                newName: "DateOfMOTExpiry");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateOfAscription",
                table: "VehicleLookups",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DateOfMOTExpiry",
                table: "VehicleLookups",
                newName: "MOTExpiryDate");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateOfAscription",
                table: "VehicleLookups",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);
        }
    }
}
