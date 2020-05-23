using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AndyTipsterPro.Migrations
{
    public partial class AddingManualSubscriptinFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CanSeeComboPackage",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CanSeeElitePackage",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CanSeeUKRacingPackage",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ManualComboPackageAccessExpiresAt",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ManualElitePackageAccessExpiresAt",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ManualUKRacingPackageAccessExpiresAt",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CanSeeComboPackage",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "CanSeeElitePackage",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "CanSeeUKRacingPackage",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ManualComboPackageAccessExpiresAt",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ManualElitePackageAccessExpiresAt",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ManualUKRacingPackageAccessExpiresAt",
                table: "AspNetUsers");
        }
    }
}
