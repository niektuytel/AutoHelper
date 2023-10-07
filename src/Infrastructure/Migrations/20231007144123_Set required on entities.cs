using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace AutoHelper.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Setrequiredonentities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Point>(
                name: "Location",
                table: "GarageLookups",
                type: "geography",
                nullable: false,
                oldClrType: typeof(Point),
                oldType: "geography",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Point>(
                name: "Location",
                table: "GarageLookups",
                type: "geography",
                nullable: true,
                oldClrType: typeof(Point),
                oldType: "geography");
        }
    }
}
