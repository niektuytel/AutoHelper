using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoHelper.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class changemodelalittle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Garages_GarageBankingDetailsItem_BankingDetailsId",
                table: "Garages");

            migrationBuilder.DropTable(
                name: "GarageBankingDetailsItem");

            migrationBuilder.DropIndex(
                name: "IX_Garages_BankingDetailsId",
                table: "Garages");

            migrationBuilder.DropColumn(
                name: "Created",
                table: "GarageServices");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "GarageServices");

            migrationBuilder.DropColumn(
                name: "LastModified",
                table: "GarageServices");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "GarageServices");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "GarageServices");

            migrationBuilder.DropColumn(
                name: "BankingDetailsId",
                table: "Garages");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "GarageServices",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<int>(
                name: "DurationInMinutes",
                table: "GarageServices",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_GarageServices_GarageId",
                table: "GarageServices",
                column: "GarageId");

            migrationBuilder.AddForeignKey(
                name: "FK_GarageServices_Garages_GarageId",
                table: "GarageServices",
                column: "GarageId",
                principalTable: "Garages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GarageServices_Garages_GarageId",
                table: "GarageServices");

            migrationBuilder.DropIndex(
                name: "IX_GarageServices_GarageId",
                table: "GarageServices");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "GarageServices",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DurationInMinutes",
                table: "GarageServices",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "GarageServices",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "GarageServices",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModified",
                table: "GarageServices",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                table: "GarageServices",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "GarageServices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "BankingDetailsId",
                table: "Garages",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "GarageBankingDetailsItem",
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
                    table.PrimaryKey("PK_GarageBankingDetailsItem", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Garages_BankingDetailsId",
                table: "Garages",
                column: "BankingDetailsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Garages_GarageBankingDetailsItem_BankingDetailsId",
                table: "Garages",
                column: "BankingDetailsId",
                principalTable: "GarageBankingDetailsItem",
                principalColumn: "Id");
        }
    }
}
