using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoHelper.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class placeimageonlargedatacolumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstPlacePhoto",
                table: "GarageLookups");

            migrationBuilder.DropColumn(
                name: "HasBestPrice",
                table: "GarageLookups");

            migrationBuilder.AddColumn<string>(
                name: "FirstPlacePhoto",
                table: "GarageLookupLargeItem",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstPlacePhoto",
                table: "GarageLookupLargeItem");

            migrationBuilder.AddColumn<string>(
                name: "FirstPlacePhoto",
                table: "GarageLookups",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HasBestPrice",
                table: "GarageLookups",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
