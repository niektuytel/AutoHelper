using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoHelper.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Changegaragestructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Conversations_GarageLookups_RelatedGarageLookupIdentifier",
                table: "Conversations");

            migrationBuilder.DropForeignKey(
                name: "FK_Garages_GarageBankingDetailsItem_BankingDetailsId",
                table: "Garages");

            migrationBuilder.DropForeignKey(
                name: "FK_Garages_GarageLocationItem_LocationId",
                table: "Garages");

            migrationBuilder.DropTable(
                name: "GarageLocationItem");

            migrationBuilder.DropIndex(
                name: "IX_Garages_LocationId",
                table: "Garages");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Garages");

            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "Garages");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Garages");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Garages");

            migrationBuilder.DropColumn(
                name: "WhatsAppNumber",
                table: "Garages");

            migrationBuilder.DropColumn(
                name: "ConversationContactType",
                table: "GarageLookups");

            migrationBuilder.RenameColumn(
                name: "ConversationContactIdentifier",
                table: "GarageLookups",
                newName: "ConversationContactWhatsappNumber");

            migrationBuilder.RenameColumn(
                name: "RelatedGarageLookupIdentifier",
                table: "Conversations",
                newName: "GarageLookupIdentifier");

            migrationBuilder.RenameIndex(
                name: "IX_Conversations_RelatedGarageLookupIdentifier",
                table: "Conversations",
                newName: "IX_Conversations_GarageLookupIdentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "BankingDetailsId",
                table: "Garages",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<string>(
                name: "GarageLookupIdentifier",
                table: "Garages",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ConversationContactEmail",
                table: "GarageLookups",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "GarageItemId",
                table: "Conversations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Garages_GarageLookupIdentifier",
                table: "Garages",
                column: "GarageLookupIdentifier");

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_GarageItemId",
                table: "Conversations",
                column: "GarageItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_Conversations_GarageLookups_GarageLookupIdentifier",
                table: "Conversations",
                column: "GarageLookupIdentifier",
                principalTable: "GarageLookups",
                principalColumn: "Identifier",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Conversations_Garages_GarageItemId",
                table: "Conversations",
                column: "GarageItemId",
                principalTable: "Garages",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Garages_GarageBankingDetailsItem_BankingDetailsId",
                table: "Garages",
                column: "BankingDetailsId",
                principalTable: "GarageBankingDetailsItem",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Garages_GarageLookups_GarageLookupIdentifier",
                table: "Garages",
                column: "GarageLookupIdentifier",
                principalTable: "GarageLookups",
                principalColumn: "Identifier",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Conversations_GarageLookups_GarageLookupIdentifier",
                table: "Conversations");

            migrationBuilder.DropForeignKey(
                name: "FK_Conversations_Garages_GarageItemId",
                table: "Conversations");

            migrationBuilder.DropForeignKey(
                name: "FK_Garages_GarageBankingDetailsItem_BankingDetailsId",
                table: "Garages");

            migrationBuilder.DropForeignKey(
                name: "FK_Garages_GarageLookups_GarageLookupIdentifier",
                table: "Garages");

            migrationBuilder.DropIndex(
                name: "IX_Garages_GarageLookupIdentifier",
                table: "Garages");

            migrationBuilder.DropIndex(
                name: "IX_Conversations_GarageItemId",
                table: "Conversations");

            migrationBuilder.DropColumn(
                name: "GarageLookupIdentifier",
                table: "Garages");

            migrationBuilder.DropColumn(
                name: "ConversationContactEmail",
                table: "GarageLookups");

            migrationBuilder.DropColumn(
                name: "GarageItemId",
                table: "Conversations");

            migrationBuilder.RenameColumn(
                name: "ConversationContactWhatsappNumber",
                table: "GarageLookups",
                newName: "ConversationContactIdentifier");

            migrationBuilder.RenameColumn(
                name: "GarageLookupIdentifier",
                table: "Conversations",
                newName: "RelatedGarageLookupIdentifier");

            migrationBuilder.RenameIndex(
                name: "IX_Conversations_GarageLookupIdentifier",
                table: "Conversations",
                newName: "IX_Conversations_RelatedGarageLookupIdentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "BankingDetailsId",
                table: "Garages",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Garages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "LocationId",
                table: "Garages",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Garages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Garages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "WhatsAppNumber",
                table: "Garages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ConversationContactType",
                table: "GarageLookups",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "GarageLocationItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Latitude = table.Column<float>(type: "real", nullable: false),
                    Longitude = table.Column<float>(type: "real", nullable: false),
                    PostalCode = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GarageLocationItem", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Garages_LocationId",
                table: "Garages",
                column: "LocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Conversations_GarageLookups_RelatedGarageLookupIdentifier",
                table: "Conversations",
                column: "RelatedGarageLookupIdentifier",
                principalTable: "GarageLookups",
                principalColumn: "Identifier");

            migrationBuilder.AddForeignKey(
                name: "FK_Garages_GarageBankingDetailsItem_BankingDetailsId",
                table: "Garages",
                column: "BankingDetailsId",
                principalTable: "GarageBankingDetailsItem",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Garages_GarageLocationItem_LocationId",
                table: "Garages",
                column: "LocationId",
                principalTable: "GarageLocationItem",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
