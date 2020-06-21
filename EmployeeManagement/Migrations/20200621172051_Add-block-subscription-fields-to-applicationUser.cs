using Microsoft.EntityFrameworkCore.Migrations;

namespace AndyTipsterPro.Migrations
{
    public partial class AddblocksubscriptionfieldstoapplicationUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "BlockComboPackage",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "BlockElitePackage",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "BlockUKRacingPackage",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BlockComboPackage",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "BlockElitePackage",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "BlockUKRacingPackage",
                table: "AspNetUsers");
        }
    }
}
