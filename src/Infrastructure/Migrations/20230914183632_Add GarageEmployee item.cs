using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoHelper.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddGarageEmployeeitem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GarageEmployeeItem_Garages_GarageItemId",
                table: "GarageEmployeeItem");

            migrationBuilder.DropForeignKey(
                name: "FK_GarageServices_Garages_GarageItemId",
                table: "GarageServices");

            migrationBuilder.DropIndex(
                name: "IX_GarageServices_GarageItemId",
                table: "GarageServices");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GarageEmployeeItem",
                table: "GarageEmployeeItem");

            migrationBuilder.DropIndex(
                name: "IX_GarageEmployeeItem_GarageItemId",
                table: "GarageEmployeeItem");

            migrationBuilder.DropColumn(
                name: "GarageItemId",
                table: "GarageServices");

            migrationBuilder.DropColumn(
                name: "Created",
                table: "ContactItem");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "ContactItem");

            migrationBuilder.DropColumn(
                name: "LastModified",
                table: "ContactItem");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "ContactItem");

            migrationBuilder.DropColumn(
                name: "Responsibility",
                table: "ContactItem");

            migrationBuilder.DropColumn(
                name: "DateOfHire",
                table: "GarageEmployeeItem");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "GarageEmployeeItem");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "GarageEmployeeItem");

            migrationBuilder.DropColumn(
                name: "GarageItemId",
                table: "GarageEmployeeItem");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "GarageEmployeeItem");

            migrationBuilder.RenameTable(
                name: "GarageEmployeeItem",
                newName: "GarageEmployees");

            migrationBuilder.RenameColumn(
                name: "Position",
                table: "GarageEmployees",
                newName: "UserId");

            migrationBuilder.AlterColumn<int>(
                name: "MaxAutomaticPlannedDeliveries",
                table: "GarageServicesSettingsItem",
                type: "int",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddColumn<Guid>(
                name: "ContactId",
                table: "GarageEmployees",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "GarageId",
                table: "GarageEmployees",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_GarageEmployees",
                table: "GarageEmployees",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "GarageEmployeeWorkExperienceItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ServiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GarageEmployeeItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GarageEmployeeWorkExperienceItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GarageEmployeeWorkExperienceItem_GarageEmployees_GarageEmployeeItemId",
                        column: x => x.GarageEmployeeItemId,
                        principalTable: "GarageEmployees",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "GarageEmployeeWorkScheduleItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ServiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TaskDescription = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    StartDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsAllDayEvent = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    GarageEmployeeItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GarageEmployeeWorkScheduleItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GarageEmployeeWorkScheduleItem_GarageEmployees_GarageEmployeeItemId",
                        column: x => x.GarageEmployeeItemId,
                        principalTable: "GarageEmployees",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_GarageEmployees_ContactId",
                table: "GarageEmployees",
                column: "ContactId");

            migrationBuilder.CreateIndex(
                name: "IX_GarageEmployeeWorkExperienceItem_GarageEmployeeItemId",
                table: "GarageEmployeeWorkExperienceItem",
                column: "GarageEmployeeItemId");

            migrationBuilder.CreateIndex(
                name: "IX_GarageEmployeeWorkScheduleItem_GarageEmployeeItemId",
                table: "GarageEmployeeWorkScheduleItem",
                column: "GarageEmployeeItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_GarageEmployees_ContactItem_ContactId",
                table: "GarageEmployees",
                column: "ContactId",
                principalTable: "ContactItem",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GarageEmployees_ContactItem_ContactId",
                table: "GarageEmployees");

            migrationBuilder.DropTable(
                name: "GarageEmployeeWorkExperienceItem");

            migrationBuilder.DropTable(
                name: "GarageEmployeeWorkScheduleItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GarageEmployees",
                table: "GarageEmployees");

            migrationBuilder.DropIndex(
                name: "IX_GarageEmployees_ContactId",
                table: "GarageEmployees");

            migrationBuilder.DropColumn(
                name: "ContactId",
                table: "GarageEmployees");

            migrationBuilder.DropColumn(
                name: "GarageId",
                table: "GarageEmployees");

            migrationBuilder.RenameTable(
                name: "GarageEmployees",
                newName: "GarageEmployeeItem");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "GarageEmployeeItem",
                newName: "Position");

            migrationBuilder.AlterColumn<decimal>(
                name: "MaxAutomaticPlannedDeliveries",
                table: "GarageServicesSettingsItem",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<Guid>(
                name: "GarageItemId",
                table: "GarageServices",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "ContactItem",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "ContactItem",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModified",
                table: "ContactItem",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                table: "ContactItem",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Responsibility",
                table: "ContactItem",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfHire",
                table: "GarageEmployeeItem",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "GarageEmployeeItem",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "GarageEmployeeItem",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "GarageItemId",
                table: "GarageEmployeeItem",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "GarageEmployeeItem",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GarageEmployeeItem",
                table: "GarageEmployeeItem",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_GarageServices_GarageItemId",
                table: "GarageServices",
                column: "GarageItemId");

            migrationBuilder.CreateIndex(
                name: "IX_GarageEmployeeItem_GarageItemId",
                table: "GarageEmployeeItem",
                column: "GarageItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_GarageEmployeeItem_Garages_GarageItemId",
                table: "GarageEmployeeItem",
                column: "GarageItemId",
                principalTable: "Garages",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GarageServices_Garages_GarageItemId",
                table: "GarageServices",
                column: "GarageItemId",
                principalTable: "Garages",
                principalColumn: "Id");
        }
    }
}
