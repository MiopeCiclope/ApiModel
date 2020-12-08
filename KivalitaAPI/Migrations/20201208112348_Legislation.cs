using Microsoft.EntityFrameworkCore.Migrations;

namespace KivalitaAPI.Migrations
{
    public partial class Legislation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "isLegislation",
                table: "Post",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isLegislation",
                table: "Post");
        }
    }
}
