using Microsoft.EntityFrameworkCore.Migrations;

namespace KivalitaAPI.Migrations
{
    public partial class GuessFlag : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "DidGuessEmail",
                table: "LeadsHistory",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "DidGuessEmail",
                table: "Leads",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DidGuessEmail",
                table: "LeadsHistory");

            migrationBuilder.DropColumn(
                name: "DidGuessEmail",
                table: "Leads");
        }
    }
}
