using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace AutoHelper.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class garagelocationnullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Geometry>(
                name: "Location",
                table: "GarageLookups",
                type: "geography",
                nullable: true,
                oldClrType: typeof(Geometry),
                oldType: "geography");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Geometry>(
                name: "Location",
                table: "GarageLookups",
                type: "geography",
                nullable: false,
                oldClrType: typeof(Geometry),
                oldType: "geography",
                oldNullable: true);
        }
    }
}
