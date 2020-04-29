using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AndyTipsterPro.Migrations
{
    public partial class addingTestimonials : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Questions");

            migrationBuilder.CreateTable(
                name: "Testimonial",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Description = table.Column<string>(maxLength: 1000, nullable: false),
                    Customer = table.Column<string>(maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Testimonial", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Testimonial");

            migrationBuilder.CreateTable(
                name: "Questions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Answer = table.Column<string>(maxLength: 2000, nullable: false),
                    Question = table.Column<string>(maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questions", x => x.Id);
                });
        }
    }
}
