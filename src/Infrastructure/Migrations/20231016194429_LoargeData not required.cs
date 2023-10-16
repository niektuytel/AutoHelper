using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoHelper.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class LoargeDatanotrequired : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GarageLookups_GarageLookupLargeItem_LargeDataId",
                table: "GarageLookups");

            migrationBuilder.AlterColumn<Guid>(
                name: "LargeDataId",
                table: "GarageLookups",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_GarageLookups_GarageLookupLargeItem_LargeDataId",
                table: "GarageLookups",
                column: "LargeDataId",
                principalTable: "GarageLookupLargeItem",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GarageLookups_GarageLookupLargeItem_LargeDataId",
                table: "GarageLookups");

            migrationBuilder.AlterColumn<Guid>(
                name: "LargeDataId",
                table: "GarageLookups",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_GarageLookups_GarageLookupLargeItem_LargeDataId",
                table: "GarageLookups",
                column: "LargeDataId",
                principalTable: "GarageLookupLargeItem",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
