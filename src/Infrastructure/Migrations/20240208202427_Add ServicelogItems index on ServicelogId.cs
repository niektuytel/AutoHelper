using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoHelper.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddServicelogItemsindexonServicelogId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_VehicleTimelineItems_VehicleServiceLogId",
                table: "VehicleTimelineItems",
                column: "VehicleServiceLogId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_VehicleTimelineItems_VehicleServiceLogId",
                table: "VehicleTimelineItems");
        }
    }
}
