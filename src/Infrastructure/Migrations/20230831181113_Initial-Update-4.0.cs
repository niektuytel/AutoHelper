using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoHelper.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialUpdate40 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Garages_BusinessOwnerItem_BusinessOwnerId",
                table: "Garages");

            migrationBuilder.DropTable(
                name: "BusinessOwnerItem");

            migrationBuilder.DropIndex(
                name: "IX_Garages_BusinessOwnerId",
                table: "Garages");

            migrationBuilder.DropColumn(
                name: "Created",
                table: "LocationItem");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "LocationItem");

            migrationBuilder.DropColumn(
                name: "LastModified",
                table: "LocationItem");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "LocationItem");

            migrationBuilder.DropColumn(
                name: "BusinessOwnerId",
                table: "Garages");

            migrationBuilder.DropColumn(
                name: "Created",
                table: "BankingInfoItem");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "BankingInfoItem");

            migrationBuilder.DropColumn(
                name: "LastModified",
                table: "BankingInfoItem");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "BankingInfoItem");

            migrationBuilder.RenameColumn(
                name: "SWIFTCode",
                table: "BankingInfoItem",
                newName: "KvKNumber");

            migrationBuilder.RenameColumn(
                name: "AccountNumber",
                table: "BankingInfoItem",
                newName: "AccountHolderName");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Garages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Garages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "WhatsAppNumber",
                table: "Garages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "Garages");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Garages");

            migrationBuilder.DropColumn(
                name: "WhatsAppNumber",
                table: "Garages");

            migrationBuilder.RenameColumn(
                name: "KvKNumber",
                table: "BankingInfoItem",
                newName: "SWIFTCode");

            migrationBuilder.RenameColumn(
                name: "AccountHolderName",
                table: "BankingInfoItem",
                newName: "AccountNumber");

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "LocationItem",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "LocationItem",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModified",
                table: "LocationItem",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                table: "LocationItem",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "BusinessOwnerId",
                table: "Garages",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "BankingInfoItem",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "BankingInfoItem",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModified",
                table: "BankingInfoItem",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                table: "BankingInfoItem",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BusinessOwnerItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessOwnerItem", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Garages_BusinessOwnerId",
                table: "Garages",
                column: "BusinessOwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Garages_BusinessOwnerItem_BusinessOwnerId",
                table: "Garages",
                column: "BusinessOwnerId",
                principalTable: "BusinessOwnerItem",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
