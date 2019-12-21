using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AndyTipsterPro.Migrations
{
    public partial class Adding_models : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PayPalAgreementId",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "SendEmails",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "SubscriptionDescription",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SubscriptionEmail",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SubscriptionFirstName",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SubscriptionId",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SubscriptionLastName",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SubscriptionPostalCode",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SubscriptionState",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Abouts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Header = table.Column<string>(maxLength: 200, nullable: false),
                    FirstTopParagraph = table.Column<string>(maxLength: 2000, nullable: false),
                    SecondTopParagraph = table.Column<string>(maxLength: 2000, nullable: false),
                    PackagesTitle = table.Column<string>(nullable: true),
                    PackageOneHeading = table.Column<string>(nullable: true),
                    PackageOneDetails = table.Column<string>(maxLength: 2000, nullable: false),
                    PackageTwoHeading = table.Column<string>(nullable: true),
                    PackageTWoDetails = table.Column<string>(maxLength: 2000, nullable: false),
                    PackageThreeHeading = table.Column<string>(nullable: true),
                    PackageThreeDetails = table.Column<string>(maxLength: 2000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Abouts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LandingPages",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    LandingPageHtml = table.Column<string>(maxLength: 12000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LandingPages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Questions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Question = table.Column<string>(maxLength: 250, nullable: false),
                    Answer = table.Column<string>(maxLength: 2000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Subscriptions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FirstName = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    StartDate = table.Column<DateTime>(nullable: false),
                    PayPalPlanId = table.Column<string>(nullable: true),
                    PayPalAgreementToken = table.Column<string>(nullable: true),
                    PayPalAgreementId = table.Column<string>(nullable: true),
                    PayPalPlanName = table.Column<string>(nullable: true),
                    PayPalPaymentEmail = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscriptions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tips",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AndyTipsterTips = table.Column<string>(maxLength: 2000, nullable: false),
                    IrishHorseTips = table.Column<string>(maxLength: 2000, nullable: false),
                    UltimateTips = table.Column<string>(maxLength: 2000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tips", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Abouts");

            migrationBuilder.DropTable(
                name: "LandingPages");

            migrationBuilder.DropTable(
                name: "Questions");

            migrationBuilder.DropTable(
                name: "Subscriptions");

            migrationBuilder.DropTable(
                name: "Tips");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "PayPalAgreementId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "SendEmails",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "SubscriptionDescription",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "SubscriptionEmail",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "SubscriptionFirstName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "SubscriptionId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "SubscriptionLastName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "SubscriptionPostalCode",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "SubscriptionState",
                table: "AspNetUsers");
        }
    }
}
