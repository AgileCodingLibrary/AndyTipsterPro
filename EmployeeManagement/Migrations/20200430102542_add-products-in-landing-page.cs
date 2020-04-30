using Microsoft.EntityFrameworkCore.Migrations;

namespace AndyTipsterPro.Migrations
{
    public partial class addproductsinlandingpage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LandingPageId",
                table: "Products",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_LandingPageId",
                table: "Products",
                column: "LandingPageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_LandingPages_LandingPageId",
                table: "Products",
                column: "LandingPageId",
                principalTable: "LandingPages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_LandingPages_LandingPageId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_LandingPageId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "LandingPageId",
                table: "Products");
        }
    }
}
