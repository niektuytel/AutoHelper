using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoHelper.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCommit22 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "BankingDetailsId",
                table: "Garages",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "BankingInfoItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BankName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AccountNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IBAN = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SWIFTCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankingInfoItem", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VehicleOwnerItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleOwnerItem", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VehicleItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LicensePlate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Model = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Brand = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RegistrationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    VehicleOwnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GarageItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
                name: "IX_Garages_BankingDetailsId",
                table: "Garages",
                column: "BankingDetailsId");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleItem_GarageItemId",
                table: "VehicleItem",
                column: "GarageItemId");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleItem_VehicleOwnerId",
                table: "VehicleItem",
                column: "VehicleOwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Garages_BankingInfoItem_BankingDetailsId",
                table: "Garages",
                column: "BankingDetailsId",
                principalTable: "BankingInfoItem",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Garages_BankingInfoItem_BankingDetailsId",
                table: "Garages");

            migrationBuilder.DropTable(
                name: "BankingInfoItem");

            migrationBuilder.DropTable(
                name: "VehicleItem");

            migrationBuilder.DropTable(
                name: "VehicleOwnerItem");

            migrationBuilder.DropIndex(
                name: "IX_Garages_BankingDetailsId",
                table: "Garages");

            migrationBuilder.DropColumn(
                name: "BankingDetailsId",
                table: "Garages");
        }
    }
}
