using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoHelper.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class setforeignkeychange3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Conversations_RelatedVehicleLookupId",
                table: "Conversations",
                column: "RelatedVehicleLookupId");

            migrationBuilder.AddForeignKey(
                name: "FK_Conversations_VehicleLookups_RelatedVehicleLookupId",
                table: "Conversations",
                column: "RelatedVehicleLookupId",
                principalTable: "VehicleLookups",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Conversations_VehicleLookups_RelatedVehicleLookupId",
                table: "Conversations");

            migrationBuilder.DropIndex(
                name: "IX_Conversations_RelatedVehicleLookupId",
                table: "Conversations");
        }
    }
}
