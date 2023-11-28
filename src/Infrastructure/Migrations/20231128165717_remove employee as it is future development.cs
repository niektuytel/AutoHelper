using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoHelper.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class removeemployeeasitisfuturedevelopment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GarageEmployeeWorkExperienceItems");

            migrationBuilder.DropTable(
                name: "GarageEmployeeWorkSchemaItems");

            migrationBuilder.DropTable(
                name: "GarageEmployees");

            migrationBuilder.DropTable(
                name: "GarageEmployeeContactItem");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GarageEmployeeContactItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GarageEmployeeContactItem", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GarageEmployees",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContactId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GarageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GarageEmployees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GarageEmployees_GarageEmployeeContactItem_ContactId",
                        column: x => x.ContactId,
                        principalTable: "GarageEmployeeContactItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GarageEmployees_Garages_GarageId",
                        column: x => x.GarageId,
                        principalTable: "Garages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GarageEmployeeWorkExperienceItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GarageEmployeeItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    GarageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ServiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GarageEmployeeWorkExperienceItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GarageEmployeeWorkExperienceItems_GarageEmployees_GarageEmployeeItemId",
                        column: x => x.GarageEmployeeItemId,
                        principalTable: "GarageEmployees",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "GarageEmployeeWorkSchemaItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DayOfWeek = table.Column<int>(type: "int", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GarageEmployeeItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    WeekOfYear = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GarageEmployeeWorkSchemaItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GarageEmployeeWorkSchemaItems_GarageEmployees_GarageEmployeeItemId",
                        column: x => x.GarageEmployeeItemId,
                        principalTable: "GarageEmployees",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_GarageEmployees_ContactId",
                table: "GarageEmployees",
                column: "ContactId");

            migrationBuilder.CreateIndex(
                name: "IX_GarageEmployees_GarageId",
                table: "GarageEmployees",
                column: "GarageId");

            migrationBuilder.CreateIndex(
                name: "IX_GarageEmployeeWorkExperienceItems_GarageEmployeeItemId",
                table: "GarageEmployeeWorkExperienceItems",
                column: "GarageEmployeeItemId");

            migrationBuilder.CreateIndex(
                name: "IX_GarageEmployeeWorkSchemaItems_GarageEmployeeItemId",
                table: "GarageEmployeeWorkSchemaItems",
                column: "GarageEmployeeItemId");
        }
    }
}
