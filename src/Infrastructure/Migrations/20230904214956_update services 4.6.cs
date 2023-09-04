using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoHelper.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateservices46 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GarageServiceItem_Garages_GarageItemId",
                table: "GarageServiceItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GarageServiceItem",
                table: "GarageServiceItem");

            migrationBuilder.RenameTable(
                name: "GarageServiceItem",
                newName: "GarageServices");

            migrationBuilder.RenameColumn(
                name: "GarageItemId",
                table: "GarageServices",
                newName: "GarageId");

            migrationBuilder.RenameIndex(
                name: "IX_GarageServiceItem_GarageItemId",
                table: "GarageServices",
                newName: "IX_GarageServices_GarageId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GarageServices",
                table: "GarageServices",
                column: "Id");

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

            migrationBuilder.DropPrimaryKey(
                name: "PK_GarageServices",
                table: "GarageServices");

            migrationBuilder.RenameTable(
                name: "GarageServices",
                newName: "GarageServiceItem");

            migrationBuilder.RenameColumn(
                name: "GarageId",
                table: "GarageServiceItem",
                newName: "GarageItemId");

            migrationBuilder.RenameIndex(
                name: "IX_GarageServices_GarageId",
                table: "GarageServiceItem",
                newName: "IX_GarageServiceItem_GarageItemId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GarageServiceItem",
                table: "GarageServiceItem",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GarageServiceItem_Garages_GarageItemId",
                table: "GarageServiceItem",
                column: "GarageItemId",
                principalTable: "Garages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
