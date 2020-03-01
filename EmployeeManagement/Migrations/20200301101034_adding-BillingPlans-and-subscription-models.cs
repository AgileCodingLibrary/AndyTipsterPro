using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AndyTipsterPro.Migrations
{
    public partial class addingBillingPlansandsubscriptionmodels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BillingPlans",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PayPalPlanId = table.Column<string>(nullable: true),
                    Type = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Amount = table.Column<decimal>(nullable: false),
                    State = table.Column<string>(nullable: true),
                    PaymentFrequency = table.Column<string>(nullable: true),
                    PaymentInterval = table.Column<string>(nullable: true),
                    ReturnURL = table.Column<string>(nullable: true),
                    CancelURL = table.Column<string>(nullable: true),
                    CreateTime = table.Column<string>(nullable: true),
                    UpdateTime = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BillingPlans", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BillingPlans");
        }
    }
}
