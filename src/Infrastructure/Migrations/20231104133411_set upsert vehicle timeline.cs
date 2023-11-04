using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace AutoHelper.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class setupsertvehicletimeline : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "MOTExpiryDate",
                table: "VehicleLookups",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<Geometry>(
                name: "Location",
                table: "VehicleLookups",
                type: "geography",
                nullable: true,
                oldClrType: typeof(Geometry),
                oldType: "geography");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfAscription",
                table: "VehicleLookups",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "VehicleTimelineItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    ExtraDataTableJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VehicleLookupItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleTimelineItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VehicleTimelineItem_VehicleLookups_VehicleLookupItemId",
                        column: x => x.VehicleLookupItemId,
                        principalTable: "VehicleLookups",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_VehicleTimelineItem_VehicleLookupItemId",
                table: "VehicleTimelineItem",
                column: "VehicleLookupItemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VehicleTimelineItem");

            migrationBuilder.DropColumn(
                name: "DateOfAscription",
                table: "VehicleLookups");

            migrationBuilder.AlterColumn<DateTime>(
                name: "MOTExpiryDate",
                table: "VehicleLookups",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<Geometry>(
                name: "Location",
                table: "VehicleLookups",
                type: "geography",
                nullable: false,
                oldClrType: typeof(Geometry),
                oldType: "geography",
                oldNullable: true);
        }
    }
}
