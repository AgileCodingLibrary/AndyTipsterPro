using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AndyTipsterPro.Migrations
{
    public partial class Addingusermultiplesubscriptions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PayPalAgreementId",
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

            migrationBuilder.CreateTable(
                name: "UserSubscriptions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PayPalAgreementId = table.Column<string>(nullable: true),
                    PayPalPlanId = table.Column<string>(nullable: true),
                    SubscriptionId = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    State = table.Column<string>(nullable: true),
                    StartDate = table.Column<DateTime>(nullable: false),
                    PayerEmail = table.Column<string>(nullable: true),
                    PayerFirstName = table.Column<string>(nullable: true),
                    PayerLastName = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSubscriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserSubscriptions_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserSubscriptions_UserId",
                table: "UserSubscriptions",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserSubscriptions");

            migrationBuilder.AddColumn<string>(
                name: "PayPalAgreementId",
                table: "AspNetUsers",
                nullable: true);

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
        }
    }
}
