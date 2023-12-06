using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoHelper.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Changegaragelookupservices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "KnownServicesString",
                table: "GarageLookups");

            migrationBuilder.CreateTable(
                name: "GarageLookupServices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GarageLookupIdentifier = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    VehicleType = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExpectedNextDateIsRequired = table.Column<bool>(type: "bit", nullable: false),
                    ExpectedNextOdometerReadingIsRequired = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GarageLookupServices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GarageLookupServices_GarageLookups_GarageLookupIdentifier",
                        column: x => x.GarageLookupIdentifier,
                        principalTable: "GarageLookups",
                        principalColumn: "Identifier",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GarageLookupServices_GarageLookupIdentifier",
                table: "GarageLookupServices",
                column: "GarageLookupIdentifier");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GarageLookupServices");

            migrationBuilder.AddColumn<string>(
                name: "KnownServicesString",
                table: "GarageLookups",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
