using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoHelper.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Cleanupvehiclelookups : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VehicleServiceLogs_VehicleServiceLogVerificationItem_VerificationId",
                table: "VehicleServiceLogs");

            migrationBuilder.DropTable(
                name: "VehicleServiceLogVerificationItem");

            migrationBuilder.DropIndex(
                name: "IX_VehicleServiceLogs_VerificationId",
                table: "VehicleServiceLogs");

            migrationBuilder.DropColumn(
                name: "VerificationId",
                table: "VehicleServiceLogs");

            migrationBuilder.RenameColumn(
                name: "WhatsappNumber",
                table: "VehicleLookups",
                newName: "ReporterWhatsappNumber");

            migrationBuilder.RenameColumn(
                name: "PhoneNumber",
                table: "VehicleLookups",
                newName: "ReporterPhoneNumber");

            migrationBuilder.RenameColumn(
                name: "EmailAddress",
                table: "VehicleLookups",
                newName: "ReporterEmailAddress");

            migrationBuilder.AddColumn<string>(
                name: "ReporterEmailAddress",
                table: "VehicleServiceLogs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReporterName",
                table: "VehicleServiceLogs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ReporterPhoneNumber",
                table: "VehicleServiceLogs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "VehicleServiceLogs",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReporterEmailAddress",
                table: "VehicleServiceLogs");

            migrationBuilder.DropColumn(
                name: "ReporterName",
                table: "VehicleServiceLogs");

            migrationBuilder.DropColumn(
                name: "ReporterPhoneNumber",
                table: "VehicleServiceLogs");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "VehicleServiceLogs");

            migrationBuilder.RenameColumn(
                name: "ReporterWhatsappNumber",
                table: "VehicleLookups",
                newName: "WhatsappNumber");

            migrationBuilder.RenameColumn(
                name: "ReporterPhoneNumber",
                table: "VehicleLookups",
                newName: "PhoneNumber");

            migrationBuilder.RenameColumn(
                name: "ReporterEmailAddress",
                table: "VehicleLookups",
                newName: "EmailAddress");

            migrationBuilder.AddColumn<Guid>(
                name: "VerificationId",
                table: "VehicleServiceLogs",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "VehicleServiceLogVerificationItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmailAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleServiceLogVerificationItem", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VehicleServiceLogs_VerificationId",
                table: "VehicleServiceLogs",
                column: "VerificationId");

            migrationBuilder.AddForeignKey(
                name: "FK_VehicleServiceLogs_VehicleServiceLogVerificationItem_VerificationId",
                table: "VehicleServiceLogs",
                column: "VerificationId",
                principalTable: "VehicleServiceLogVerificationItem",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
