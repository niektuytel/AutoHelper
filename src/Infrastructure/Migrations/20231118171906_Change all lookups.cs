using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace AutoHelper.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Changealllookups : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Conversations_GarageLookups_RelatedGarageLookupId",
                table: "Conversations");

            migrationBuilder.DropForeignKey(
                name: "FK_GarageServices_VehicleServiceLogs_VehicleServiceLogItemId",
                table: "GarageServices");

            migrationBuilder.DropIndex(
                name: "IX_GarageServices_VehicleServiceLogItemId",
                table: "GarageServices");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GarageLookups",
                table: "GarageLookups");

            migrationBuilder.DropIndex(
                name: "IX_Conversations_RelatedGarageLookupId",
                table: "Conversations");

            migrationBuilder.DropColumn(
                name: "DocumentationUrl",
                table: "VehicleServiceLogs");

            migrationBuilder.DropColumn(
                name: "TotalCost",
                table: "VehicleServiceLogs");

            migrationBuilder.DropColumn(
                name: "VehicleServiceLogItemId",
                table: "GarageServices");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "GarageLookups");

            migrationBuilder.DropColumn(
                name: "FirstPlacePhoto",
                table: "GarageLookupLargeItem");

            migrationBuilder.DropColumn(
                name: "RelatedGarageLookupId",
                table: "Conversations");

            migrationBuilder.RenameColumn(
                name: "WorkDescription",
                table: "VehicleServiceLogs",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "ServiceDate",
                table: "VehicleServiceLogs",
                newName: "Date");

            migrationBuilder.RenameColumn(
                name: "PerformedBy",
                table: "VehicleServiceLogs",
                newName: "AttachedFile");

            migrationBuilder.RenameColumn(
                name: "ExpectedNextServiceDate",
                table: "VehicleServiceLogs",
                newName: "ExpectedNextDate");

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "VehicleServiceLogs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ExpectedNextOdometerReading",
                table: "VehicleServiceLogs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GarageLookupIdentifier",
                table: "VehicleServiceLogs",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "VehicleServiceLogs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "VerificationId",
                table: "VehicleServiceLogs",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<Geometry>(
                name: "Location",
                table: "GarageLookups",
                type: "geography",
                nullable: false,
                oldClrType: typeof(Geometry),
                oldType: "geography",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Identifier",
                table: "GarageLookups",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "GarageLookups",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "GarageLookups",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "GarageLookups",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageThumbnail",
                table: "GarageLookups",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModified",
                table: "GarageLookups",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                table: "GarageLookups",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RelatedGarageLookupIdentifier",
                table: "Conversations",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GarageLookups",
                table: "GarageLookups",
                column: "Identifier");

            migrationBuilder.CreateTable(
                name: "VehicleServiceLogVerificationItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailAddress = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleServiceLogVerificationItem", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VehicleServiceLogs_GarageLookupIdentifier",
                table: "VehicleServiceLogs",
                column: "GarageLookupIdentifier");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleServiceLogs_VerificationId",
                table: "VehicleServiceLogs",
                column: "VerificationId");

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_RelatedGarageLookupIdentifier",
                table: "Conversations",
                column: "RelatedGarageLookupIdentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_Conversations_GarageLookups_RelatedGarageLookupIdentifier",
                table: "Conversations",
                column: "RelatedGarageLookupIdentifier",
                principalTable: "GarageLookups",
                principalColumn: "Identifier");

            migrationBuilder.AddForeignKey(
                name: "FK_VehicleServiceLogs_GarageLookups_GarageLookupIdentifier",
                table: "VehicleServiceLogs",
                column: "GarageLookupIdentifier",
                principalTable: "GarageLookups",
                principalColumn: "Identifier",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VehicleServiceLogs_VehicleServiceLogVerificationItem_VerificationId",
                table: "VehicleServiceLogs",
                column: "VerificationId",
                principalTable: "VehicleServiceLogVerificationItem",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Conversations_GarageLookups_RelatedGarageLookupIdentifier",
                table: "Conversations");

            migrationBuilder.DropForeignKey(
                name: "FK_VehicleServiceLogs_GarageLookups_GarageLookupIdentifier",
                table: "VehicleServiceLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_VehicleServiceLogs_VehicleServiceLogVerificationItem_VerificationId",
                table: "VehicleServiceLogs");

            migrationBuilder.DropTable(
                name: "VehicleServiceLogVerificationItem");

            migrationBuilder.DropIndex(
                name: "IX_VehicleServiceLogs_GarageLookupIdentifier",
                table: "VehicleServiceLogs");

            migrationBuilder.DropIndex(
                name: "IX_VehicleServiceLogs_VerificationId",
                table: "VehicleServiceLogs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GarageLookups",
                table: "GarageLookups");

            migrationBuilder.DropIndex(
                name: "IX_Conversations_RelatedGarageLookupIdentifier",
                table: "Conversations");

            migrationBuilder.DropColumn(
                name: "ExpectedNextOdometerReading",
                table: "VehicleServiceLogs");

            migrationBuilder.DropColumn(
                name: "GarageLookupIdentifier",
                table: "VehicleServiceLogs");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "VehicleServiceLogs");

            migrationBuilder.DropColumn(
                name: "VerificationId",
                table: "VehicleServiceLogs");

            migrationBuilder.DropColumn(
                name: "Created",
                table: "GarageLookups");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "GarageLookups");

            migrationBuilder.DropColumn(
                name: "Image",
                table: "GarageLookups");

            migrationBuilder.DropColumn(
                name: "ImageThumbnail",
                table: "GarageLookups");

            migrationBuilder.DropColumn(
                name: "LastModified",
                table: "GarageLookups");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "GarageLookups");

            migrationBuilder.DropColumn(
                name: "RelatedGarageLookupIdentifier",
                table: "Conversations");

            migrationBuilder.RenameColumn(
                name: "ExpectedNextDate",
                table: "VehicleServiceLogs",
                newName: "ExpectedNextServiceDate");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "VehicleServiceLogs",
                newName: "WorkDescription");

            migrationBuilder.RenameColumn(
                name: "Date",
                table: "VehicleServiceLogs",
                newName: "ServiceDate");

            migrationBuilder.RenameColumn(
                name: "AttachedFile",
                table: "VehicleServiceLogs",
                newName: "PerformedBy");

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "VehicleServiceLogs",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "DocumentationUrl",
                table: "VehicleServiceLogs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalCost",
                table: "VehicleServiceLogs",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "VehicleServiceLogItemId",
                table: "GarageServices",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<Geometry>(
                name: "Location",
                table: "GarageLookups",
                type: "geography",
                nullable: true,
                oldClrType: typeof(Geometry),
                oldType: "geography");

            migrationBuilder.AlterColumn<string>(
                name: "Identifier",
                table: "GarageLookups",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "GarageLookups",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "FirstPlacePhoto",
                table: "GarageLookupLargeItem",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "RelatedGarageLookupId",
                table: "Conversations",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_GarageLookups",
                table: "GarageLookups",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_GarageServices_VehicleServiceLogItemId",
                table: "GarageServices",
                column: "VehicleServiceLogItemId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_GarageServices_VehicleServiceLogs_VehicleServiceLogItemId",
                table: "GarageServices",
                column: "VehicleServiceLogItemId",
                principalTable: "VehicleServiceLogs",
                principalColumn: "Id");
        }
    }
}
