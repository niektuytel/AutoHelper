using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoHelper.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeTitletoanType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Title",
                table: "GarageServices");

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "GarageServices",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "GarageServices");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "GarageServices",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
