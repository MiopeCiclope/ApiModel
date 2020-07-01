using Microsoft.EntityFrameworkCore.Migrations;

namespace KivalitaAPI.Migrations
{
    public partial class AddLinkedInPublicField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LinkedInPublic",
                table: "LeadsHistory",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LinkedInPublic",
                table: "Leads",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LinkedInPublic",
                table: "LeadsHistory");

            migrationBuilder.DropColumn(
                name: "LinkedInPublic",
                table: "Leads");
        }
    }
}
