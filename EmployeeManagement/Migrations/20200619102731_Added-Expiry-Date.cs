using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AndyTipsterPro.Migrations
{
    public partial class AddedExpiryDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiryDate",
                table: "UserSubscriptions",
                nullable: false,
                defaultValue: new DateTime(1994, 4, 4, 0, 0, 0, 0, DateTimeKind.Unspecified)); 
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpiryDate",
                table: "UserSubscriptions");
          
        }
    }
}
