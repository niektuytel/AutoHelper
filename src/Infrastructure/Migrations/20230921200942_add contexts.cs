using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoHelper.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addcontexts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GarageEmployeeWorkExperienceItem_GarageEmployees_GarageEmployeeItemId",
                table: "GarageEmployeeWorkExperienceItem");

            migrationBuilder.DropForeignKey(
                name: "FK_GarageEmployeeWorkSchemaItem_GarageEmployees_GarageEmployeeItemId",
                table: "GarageEmployeeWorkSchemaItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GarageEmployeeWorkSchemaItem",
                table: "GarageEmployeeWorkSchemaItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GarageEmployeeWorkExperienceItem",
                table: "GarageEmployeeWorkExperienceItem");

            migrationBuilder.RenameTable(
                name: "GarageEmployeeWorkSchemaItem",
                newName: "GarageEmployeeWorkSchemas");

            migrationBuilder.RenameTable(
                name: "GarageEmployeeWorkExperienceItem",
                newName: "GarageEmployeeWorkExperienceItems");

            migrationBuilder.RenameIndex(
                name: "IX_GarageEmployeeWorkSchemaItem_GarageEmployeeItemId",
                table: "GarageEmployeeWorkSchemas",
                newName: "IX_GarageEmployeeWorkSchemas_GarageEmployeeItemId");

            migrationBuilder.RenameIndex(
                name: "IX_GarageEmployeeWorkExperienceItem_GarageEmployeeItemId",
                table: "GarageEmployeeWorkExperienceItems",
                newName: "IX_GarageEmployeeWorkExperienceItems_GarageEmployeeItemId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GarageEmployeeWorkSchemas",
                table: "GarageEmployeeWorkSchemas",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GarageEmployeeWorkExperienceItems",
                table: "GarageEmployeeWorkExperienceItems",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GarageEmployeeWorkExperienceItems_GarageEmployees_GarageEmployeeItemId",
                table: "GarageEmployeeWorkExperienceItems",
                column: "GarageEmployeeItemId",
                principalTable: "GarageEmployees",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GarageEmployeeWorkSchemas_GarageEmployees_GarageEmployeeItemId",
                table: "GarageEmployeeWorkSchemas",
                column: "GarageEmployeeItemId",
                principalTable: "GarageEmployees",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GarageEmployeeWorkExperienceItems_GarageEmployees_GarageEmployeeItemId",
                table: "GarageEmployeeWorkExperienceItems");

            migrationBuilder.DropForeignKey(
                name: "FK_GarageEmployeeWorkSchemas_GarageEmployees_GarageEmployeeItemId",
                table: "GarageEmployeeWorkSchemas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GarageEmployeeWorkSchemas",
                table: "GarageEmployeeWorkSchemas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GarageEmployeeWorkExperienceItems",
                table: "GarageEmployeeWorkExperienceItems");

            migrationBuilder.RenameTable(
                name: "GarageEmployeeWorkSchemas",
                newName: "GarageEmployeeWorkSchemaItem");

            migrationBuilder.RenameTable(
                name: "GarageEmployeeWorkExperienceItems",
                newName: "GarageEmployeeWorkExperienceItem");

            migrationBuilder.RenameIndex(
                name: "IX_GarageEmployeeWorkSchemas_GarageEmployeeItemId",
                table: "GarageEmployeeWorkSchemaItem",
                newName: "IX_GarageEmployeeWorkSchemaItem_GarageEmployeeItemId");

            migrationBuilder.RenameIndex(
                name: "IX_GarageEmployeeWorkExperienceItems_GarageEmployeeItemId",
                table: "GarageEmployeeWorkExperienceItem",
                newName: "IX_GarageEmployeeWorkExperienceItem_GarageEmployeeItemId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GarageEmployeeWorkSchemaItem",
                table: "GarageEmployeeWorkSchemaItem",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GarageEmployeeWorkExperienceItem",
                table: "GarageEmployeeWorkExperienceItem",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GarageEmployeeWorkExperienceItem_GarageEmployees_GarageEmployeeItemId",
                table: "GarageEmployeeWorkExperienceItem",
                column: "GarageEmployeeItemId",
                principalTable: "GarageEmployees",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GarageEmployeeWorkSchemaItem_GarageEmployees_GarageEmployeeItemId",
                table: "GarageEmployeeWorkSchemaItem",
                column: "GarageEmployeeItemId",
                principalTable: "GarageEmployees",
                principalColumn: "Id");
        }
    }
}
