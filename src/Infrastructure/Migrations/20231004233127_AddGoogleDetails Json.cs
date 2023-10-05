using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoHelper.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddGoogleDetailsJson : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GoogleApiDetailsJson",
                table: "GarageLookups",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<float>(
                name: "Rating",
                table: "GarageLookups",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "GarageLookups",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "UserRatingsTotal",
                table: "GarageLookups",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Website",
                table: "GarageLookups",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GoogleApiDetailsJson",
                table: "GarageLookups");

            migrationBuilder.DropColumn(
                name: "Rating",
                table: "GarageLookups");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "GarageLookups");

            migrationBuilder.DropColumn(
                name: "UserRatingsTotal",
                table: "GarageLookups");

            migrationBuilder.DropColumn(
                name: "Website",
                table: "GarageLookups");
        }
    }
}
