using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoHelper.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCommit21 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Garages_LocationItem_LocationItemId",
                table: "Garages");

            migrationBuilder.RenameColumn(
                name: "LocationItemId",
                table: "Garages",
                newName: "LocationId");

            migrationBuilder.RenameIndex(
                name: "IX_Garages_LocationItemId",
                table: "Garages",
                newName: "IX_Garages_LocationId");

            migrationBuilder.AddColumn<Guid>(
                name: "BusinessOwnerId",
                table: "Garages",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "BusinessOwnerItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
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

            migrationBuilder.AddForeignKey(
                name: "FK_Garages_LocationItem_LocationId",
                table: "Garages",
                column: "LocationId",
                principalTable: "LocationItem",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Garages_BusinessOwnerItem_BusinessOwnerId",
                table: "Garages");

            migrationBuilder.DropForeignKey(
                name: "FK_Garages_LocationItem_LocationId",
                table: "Garages");

            migrationBuilder.DropTable(
                name: "BusinessOwnerItem");

            migrationBuilder.DropIndex(
                name: "IX_Garages_BusinessOwnerId",
                table: "Garages");

            migrationBuilder.DropColumn(
                name: "BusinessOwnerId",
                table: "Garages");

            migrationBuilder.RenameColumn(
                name: "LocationId",
                table: "Garages",
                newName: "LocationItemId");

            migrationBuilder.RenameIndex(
                name: "IX_Garages_LocationId",
                table: "Garages",
                newName: "IX_Garages_LocationItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_Garages_LocationItem_LocationItemId",
                table: "Garages",
                column: "LocationItemId",
                principalTable: "LocationItem",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
