using Microsoft.EntityFrameworkCore.Migrations;

namespace AndyTipsterPro.Migrations
{
    public partial class addingagainBillingPlansandsubscriptionmodels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Amount",
                table: "BillingPlans");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Amount",
                table: "BillingPlans",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
