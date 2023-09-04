using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoHelper.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class initial45 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Garages_BankingDetailsItem_BankingDetailsId",
                table: "Garages");

            migrationBuilder.DropForeignKey(
                name: "FK_Garages_LocationItem_LocationId",
                table: "Garages");

            migrationBuilder.DropTable(
                name: "BankingDetailsItem");

            migrationBuilder.DropTable(
                name: "LocationItem");

            migrationBuilder.CreateTable(
                name: "GarageBankingDetailsItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BankName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    KvKNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AccountHolderName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IBAN = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GarageBankingDetailsItem", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GarageLocationItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PostalCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Longitude = table.Column<double>(type: "float", nullable: false),
                    Latitude = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GarageLocationItem", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Garages_GarageBankingDetailsItem_BankingDetailsId",
                table: "Garages",
                column: "BankingDetailsId",
                principalTable: "GarageBankingDetailsItem",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Garages_GarageLocationItem_LocationId",
                table: "Garages",
                column: "LocationId",
                principalTable: "GarageLocationItem",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Garages_GarageBankingDetailsItem_BankingDetailsId",
                table: "Garages");

            migrationBuilder.DropForeignKey(
                name: "FK_Garages_GarageLocationItem_LocationId",
                table: "Garages");

            migrationBuilder.DropTable(
                name: "GarageBankingDetailsItem");

            migrationBuilder.DropTable(
                name: "GarageLocationItem");

            migrationBuilder.CreateTable(
                name: "BankingDetailsItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountHolderName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BankName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IBAN = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    KvKNumber = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankingDetailsItem", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LocationItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Latitude = table.Column<double>(type: "float", nullable: false),
                    Longitude = table.Column<double>(type: "float", nullable: false),
                    PostalCode = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocationItem", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Garages_BankingDetailsItem_BankingDetailsId",
                table: "Garages",
                column: "BankingDetailsId",
                principalTable: "BankingDetailsItem",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Garages_LocationItem_LocationId",
                table: "Garages",
                column: "LocationId",
                principalTable: "LocationItem",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
