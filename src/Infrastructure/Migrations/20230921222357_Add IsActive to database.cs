using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoHelper.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIsActivetodatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GarageEmployeeWorkSchemas_GarageEmployees_GarageEmployeeItemId",
                table: "GarageEmployeeWorkSchemas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GarageEmployeeWorkSchemas",
                table: "GarageEmployeeWorkSchemas");

            migrationBuilder.RenameTable(
                name: "GarageEmployeeWorkSchemas",
                newName: "GarageEmployeeWorkSchemaItems");

            migrationBuilder.RenameIndex(
                name: "IX_GarageEmployeeWorkSchemas_GarageEmployeeItemId",
                table: "GarageEmployeeWorkSchemaItems",
                newName: "IX_GarageEmployeeWorkSchemaItems_GarageEmployeeItemId");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "GarageEmployees",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_GarageEmployeeWorkSchemaItems",
                table: "GarageEmployeeWorkSchemaItems",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GarageEmployeeWorkSchemaItems_GarageEmployees_GarageEmployeeItemId",
                table: "GarageEmployeeWorkSchemaItems",
                column: "GarageEmployeeItemId",
                principalTable: "GarageEmployees",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GarageEmployeeWorkSchemaItems_GarageEmployees_GarageEmployeeItemId",
                table: "GarageEmployeeWorkSchemaItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GarageEmployeeWorkSchemaItems",
                table: "GarageEmployeeWorkSchemaItems");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "GarageEmployees");

            migrationBuilder.RenameTable(
                name: "GarageEmployeeWorkSchemaItems",
                newName: "GarageEmployeeWorkSchemas");

            migrationBuilder.RenameIndex(
                name: "IX_GarageEmployeeWorkSchemaItems_GarageEmployeeItemId",
                table: "GarageEmployeeWorkSchemas",
                newName: "IX_GarageEmployeeWorkSchemas_GarageEmployeeItemId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GarageEmployeeWorkSchemas",
                table: "GarageEmployeeWorkSchemas",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GarageEmployeeWorkSchemas_GarageEmployees_GarageEmployeeItemId",
                table: "GarageEmployeeWorkSchemas",
                column: "GarageEmployeeItemId",
                principalTable: "GarageEmployees",
                principalColumn: "Id");
        }
    }
}
