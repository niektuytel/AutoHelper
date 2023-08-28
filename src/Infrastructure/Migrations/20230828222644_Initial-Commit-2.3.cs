using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoHelper.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCommit23 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ContactItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Responsibility = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GarageItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContactItem_Garages_GarageItemId",
                        column: x => x.GarageItemId,
                        principalTable: "Garages",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "GarageEmployeeItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Position = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateOfHire = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GarageItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GarageEmployeeItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GarageEmployeeItem_Garages_GarageItemId",
                        column: x => x.GarageItemId,
                        principalTable: "Garages",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "GarageServiceItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ServiceName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GarageItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GarageServiceItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GarageServiceItem_Garages_GarageItemId",
                        column: x => x.GarageItemId,
                        principalTable: "Garages",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContactItem_GarageItemId",
                table: "ContactItem",
                column: "GarageItemId");

            migrationBuilder.CreateIndex(
                name: "IX_GarageEmployeeItem_GarageItemId",
                table: "GarageEmployeeItem",
                column: "GarageItemId");

            migrationBuilder.CreateIndex(
                name: "IX_GarageServiceItem_GarageItemId",
                table: "GarageServiceItem",
                column: "GarageItemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContactItem");

            migrationBuilder.DropTable(
                name: "GarageEmployeeItem");

            migrationBuilder.DropTable(
                name: "GarageServiceItem");
        }
    }
}
