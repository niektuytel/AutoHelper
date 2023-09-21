using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoHelper.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class changeserviceIdtotypeint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ServiceId",
                table: "GarageEmployeeWorkExperienceItems");

            migrationBuilder.AddColumn<int>(
                name: "ServiceType",
                table: "GarageEmployeeWorkExperienceItems",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ServiceType",
                table: "GarageEmployeeWorkExperienceItems");

            migrationBuilder.AddColumn<Guid>(
                name: "ServiceId",
                table: "GarageEmployeeWorkExperienceItems",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }
    }
}
