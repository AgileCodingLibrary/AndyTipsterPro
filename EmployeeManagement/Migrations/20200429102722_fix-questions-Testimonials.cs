using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AndyTipsterPro.Migrations
{
    public partial class fixquestionsTestimonials : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Testimonial",
                table: "Testimonial");

            migrationBuilder.RenameTable(
                name: "Testimonial",
                newName: "Testimonials");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Testimonials",
                table: "Testimonials",
                column: "Id");

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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Questions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Testimonials",
                table: "Testimonials");

            migrationBuilder.RenameTable(
                name: "Testimonials",
                newName: "Testimonial");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Testimonial",
                table: "Testimonial",
                column: "Id");
        }
    }
}
