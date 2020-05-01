using Microsoft.EntityFrameworkCore.Migrations;

namespace AndyTipsterPro.Migrations
{
    public partial class Rename_Tips_columns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UltimateTips",
                table: "Tips",
                newName: "UKPackageTips");

            migrationBuilder.RenameColumn(
                name: "IrishHorseTips",
                table: "Tips",
                newName: "ElitePackageTips");

            migrationBuilder.RenameColumn(
                name: "AndyTipsterTips",
                table: "Tips",
                newName: "CombinationPackageTips");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UKPackageTips",
                table: "Tips",
                newName: "UltimateTips");

            migrationBuilder.RenameColumn(
                name: "ElitePackageTips",
                table: "Tips",
                newName: "IrishHorseTips");

            migrationBuilder.RenameColumn(
                name: "CombinationPackageTips",
                table: "Tips",
                newName: "AndyTipsterTips");
        }
    }
}
