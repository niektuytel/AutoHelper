using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoHelper.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Updateconversationtable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FromEmailAddress",
                table: "Conversations");

            migrationBuilder.DropColumn(
                name: "FromWhatsappNumber",
                table: "Conversations");

            migrationBuilder.DropColumn(
                name: "ToEmailAddress",
                table: "Conversations");

            migrationBuilder.DropColumn(
                name: "ToWhatsappNumber",
                table: "Conversations");

            migrationBuilder.AddColumn<string>(
                name: "RelatedServiceTypesString",
                table: "Conversations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RelatedServiceTypesString",
                table: "Conversations");

            migrationBuilder.AddColumn<string>(
                name: "FromEmailAddress",
                table: "Conversations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FromWhatsappNumber",
                table: "Conversations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ToEmailAddress",
                table: "Conversations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ToWhatsappNumber",
                table: "Conversations",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
