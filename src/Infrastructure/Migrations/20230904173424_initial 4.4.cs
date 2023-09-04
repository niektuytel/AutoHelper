using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoHelper.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class initial44 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GarageServiceItem_Garages_GarageItemId",
                table: "GarageServiceItem");

            migrationBuilder.RenameColumn(
                name: "ServiceName",
                table: "GarageServiceItem",
                newName: "Title");

            migrationBuilder.AlterColumn<Guid>(
                name: "GarageItemId",
                table: "GarageServiceItem",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Duration",
                table: "GarageServiceItem",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "GarageServiceItem",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "GarageServiceItem",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "ServicesSettingsId",
                table: "Garages",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "GarageServicesSettingsItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaxAutomaticPlannedOrders = table.Column<int>(type: "int", nullable: false),
                    TrySendMailOnNewOrders = table.Column<bool>(type: "bit", nullable: false),
                    TrySendWhatsappMessagOnNewOrders = table.Column<bool>(type: "bit", nullable: false),
                    IsDeliveryEnabled = table.Column<bool>(type: "bit", nullable: false),
                    IsAuthohelperDeliveryEnabled = table.Column<bool>(type: "bit", nullable: false),
                    DeliveryPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MaxAutomaticPlannedDeliveries = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GarageServicesSettingsItem", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Garages_ServicesSettingsId",
                table: "Garages",
                column: "ServicesSettingsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Garages_GarageServicesSettingsItem_ServicesSettingsId",
                table: "Garages",
                column: "ServicesSettingsId",
                principalTable: "GarageServicesSettingsItem",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GarageServiceItem_Garages_GarageItemId",
                table: "GarageServiceItem",
                column: "GarageItemId",
                principalTable: "Garages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Garages_GarageServicesSettingsItem_ServicesSettingsId",
                table: "Garages");

            migrationBuilder.DropForeignKey(
                name: "FK_GarageServiceItem_Garages_GarageItemId",
                table: "GarageServiceItem");

            migrationBuilder.DropTable(
                name: "GarageServicesSettingsItem");

            migrationBuilder.DropIndex(
                name: "IX_Garages_ServicesSettingsId",
                table: "Garages");

            migrationBuilder.DropColumn(
                name: "Duration",
                table: "GarageServiceItem");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "GarageServiceItem");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "GarageServiceItem");

            migrationBuilder.DropColumn(
                name: "ServicesSettingsId",
                table: "Garages");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "GarageServiceItem",
                newName: "ServiceName");

            migrationBuilder.AlterColumn<Guid>(
                name: "GarageItemId",
                table: "GarageServiceItem",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_GarageServiceItem_Garages_GarageItemId",
                table: "GarageServiceItem",
                column: "GarageItemId",
                principalTable: "Garages",
                principalColumn: "Id");
        }
    }
}
