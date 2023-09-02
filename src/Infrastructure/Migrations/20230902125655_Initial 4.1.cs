using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoHelper.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial41 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Garages_BankingInfoItem_BankingDetailsId",
                table: "Garages");

            migrationBuilder.DropTable(
                name: "BankingInfoItem");

            migrationBuilder.CreateTable(
                name: "BankingDetailsItem",
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
                    table.PrimaryKey("PK_BankingDetailsItem", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Garages_BankingDetailsItem_BankingDetailsId",
                table: "Garages",
                column: "BankingDetailsId",
                principalTable: "BankingDetailsItem",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Garages_BankingDetailsItem_BankingDetailsId",
                table: "Garages");

            migrationBuilder.DropTable(
                name: "BankingDetailsItem");

            migrationBuilder.CreateTable(
                name: "BankingInfoItem",
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
                    table.PrimaryKey("PK_BankingInfoItem", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Garages_BankingInfoItem_BankingDetailsId",
                table: "Garages",
                column: "BankingDetailsId",
                principalTable: "BankingInfoItem",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
