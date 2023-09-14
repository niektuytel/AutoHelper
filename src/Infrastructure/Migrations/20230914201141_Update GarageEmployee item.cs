using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoHelper.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateGarageEmployeeitem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GarageEmployeeWorkScheduleItem");

            migrationBuilder.CreateTable(
                name: "GarageEmployeeWorkSchemaItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WeekOfYear = table.Column<int>(type: "int", nullable: false),
                    DayOfWeek = table.Column<int>(type: "int", nullable: false),
                    StartDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    GarageEmployeeItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GarageEmployeeWorkSchemaItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GarageEmployeeWorkSchemaItem_GarageEmployees_GarageEmployeeItemId",
                        column: x => x.GarageEmployeeItemId,
                        principalTable: "GarageEmployees",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_GarageEmployeeWorkSchemaItem_GarageEmployeeItemId",
                table: "GarageEmployeeWorkSchemaItem",
                column: "GarageEmployeeItemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GarageEmployeeWorkSchemaItem");

            migrationBuilder.CreateTable(
                name: "GarageEmployeeWorkScheduleItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EndDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GarageEmployeeItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsAllDayEvent = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ServiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TaskDescription = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
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
                name: "IX_GarageEmployeeWorkScheduleItem_GarageEmployeeItemId",
                table: "GarageEmployeeWorkScheduleItem",
                column: "GarageEmployeeItemId");
        }
    }
}
