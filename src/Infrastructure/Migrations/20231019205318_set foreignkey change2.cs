using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoHelper.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class setforeignkeychange2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Conversations_GarageLookups_GarageLookupItemId",
                table: "Conversations");

            migrationBuilder.DropIndex(
                name: "IX_Conversations_GarageLookupItemId",
                table: "Conversations");

            migrationBuilder.DropColumn(
                name: "GarageLookupItemId",
                table: "Conversations");

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_RelatedGarageLookupId",
                table: "Conversations",
                column: "RelatedGarageLookupId");

            migrationBuilder.AddForeignKey(
                name: "FK_Conversations_GarageLookups_RelatedGarageLookupId",
                table: "Conversations",
                column: "RelatedGarageLookupId",
                principalTable: "GarageLookups",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Conversations_GarageLookups_RelatedGarageLookupId",
                table: "Conversations");

            migrationBuilder.DropIndex(
                name: "IX_Conversations_RelatedGarageLookupId",
                table: "Conversations");

            migrationBuilder.AddColumn<Guid>(
                name: "GarageLookupItemId",
                table: "Conversations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_GarageLookupItemId",
                table: "Conversations",
                column: "GarageLookupItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_Conversations_GarageLookups_GarageLookupItemId",
                table: "Conversations",
                column: "GarageLookupItemId",
                principalTable: "GarageLookups",
                principalColumn: "Id");
        }
    }
}
