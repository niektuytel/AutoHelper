using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoHelper.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class renametouserId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AccountId",
                table: "GarageServices",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "AccountId",
                table: "Garages",
                newName: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "GarageServices",
                newName: "AccountId");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Garages",
                newName: "AccountId");
        }
    }
}
