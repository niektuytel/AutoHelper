using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoHelper.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CreateVehicleItemVehicleServiceLogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VehicleItem");

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "VehicleOwnerItem",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<Guid>(
                name: "VehicleOwnerLocationId",
                table: "VehicleOwnerItem",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "VehicleServiceLogItemId",
                table: "GarageServices",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<float>(
                name: "Longitude",
                table: "GarageLocationItem",
                type: "real",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<float>(
                name: "Latitude",
                table: "GarageLocationItem",
                type: "real",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.CreateTable(
                name: "VehicleLocationItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PostalCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Longitude = table.Column<float>(type: "real", nullable: false),
                    Latitude = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleLocationItem", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VehicleOwnerLocationItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PostalCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Longitude = table.Column<float>(type: "real", nullable: true),
                    Latitude = table.Column<float>(type: "real", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleOwnerLocationItem", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Vehicles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LicensePlate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MOTExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastLocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastVehicleOwnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    GarageItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vehicles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vehicles_Garages_GarageItemId",
                        column: x => x.GarageItemId,
                        principalTable: "Garages",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Vehicles_VehicleLocationItem_LastLocationId",
                        column: x => x.LastLocationId,
                        principalTable: "VehicleLocationItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Vehicles_VehicleOwnerItem_LastVehicleOwnerId",
                        column: x => x.LastVehicleOwnerId,
                        principalTable: "VehicleOwnerItem",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "VehicleServiceLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OwnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VehicleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Mileage = table.Column<int>(type: "int", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MetaData = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleServiceLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VehicleServiceLogs_VehicleOwnerItem_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "VehicleOwnerItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VehicleServiceLogs_Vehicles_VehicleId",
                        column: x => x.VehicleId,
                        principalTable: "Vehicles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VehicleOwnerItem_VehicleOwnerLocationId",
                table: "VehicleOwnerItem",
                column: "VehicleOwnerLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_GarageServices_VehicleServiceLogItemId",
                table: "GarageServices",
                column: "VehicleServiceLogItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_GarageItemId",
                table: "Vehicles",
                column: "GarageItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_LastLocationId",
                table: "Vehicles",
                column: "LastLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_LastVehicleOwnerId",
                table: "Vehicles",
                column: "LastVehicleOwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleServiceLogs_OwnerId",
                table: "VehicleServiceLogs",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleServiceLogs_VehicleId",
                table: "VehicleServiceLogs",
                column: "VehicleId");

            migrationBuilder.AddForeignKey(
                name: "FK_GarageServices_VehicleServiceLogs_VehicleServiceLogItemId",
                table: "GarageServices",
                column: "VehicleServiceLogItemId",
                principalTable: "VehicleServiceLogs",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_VehicleOwnerItem_VehicleOwnerLocationItem_VehicleOwnerLocationId",
                table: "VehicleOwnerItem",
                column: "VehicleOwnerLocationId",
                principalTable: "VehicleOwnerLocationItem",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GarageServices_VehicleServiceLogs_VehicleServiceLogItemId",
                table: "GarageServices");

            migrationBuilder.DropForeignKey(
                name: "FK_VehicleOwnerItem_VehicleOwnerLocationItem_VehicleOwnerLocationId",
                table: "VehicleOwnerItem");

            migrationBuilder.DropTable(
                name: "VehicleOwnerLocationItem");

            migrationBuilder.DropTable(
                name: "VehicleServiceLogs");

            migrationBuilder.DropTable(
                name: "Vehicles");

            migrationBuilder.DropTable(
                name: "VehicleLocationItem");

            migrationBuilder.DropIndex(
                name: "IX_VehicleOwnerItem_VehicleOwnerLocationId",
                table: "VehicleOwnerItem");

            migrationBuilder.DropIndex(
                name: "IX_GarageServices_VehicleServiceLogItemId",
                table: "GarageServices");

            migrationBuilder.DropColumn(
                name: "VehicleOwnerLocationId",
                table: "VehicleOwnerItem");

            migrationBuilder.DropColumn(
                name: "VehicleServiceLogItemId",
                table: "GarageServices");

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "VehicleOwnerItem",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Longitude",
                table: "GarageLocationItem",
                type: "float",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AlterColumn<double>(
                name: "Latitude",
                table: "GarageLocationItem",
                type: "float",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.CreateTable(
                name: "VehicleItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VehicleOwnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Brand = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GarageItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LicensePlate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Model = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RegistrationDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VehicleItem_Garages_GarageItemId",
                        column: x => x.GarageItemId,
                        principalTable: "Garages",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_VehicleItem_VehicleOwnerItem_VehicleOwnerId",
                        column: x => x.VehicleOwnerId,
                        principalTable: "VehicleOwnerItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VehicleItem_GarageItemId",
                table: "VehicleItem",
                column: "GarageItemId");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleItem_VehicleOwnerId",
                table: "VehicleItem",
                column: "VehicleOwnerId");
        }
    }
}
