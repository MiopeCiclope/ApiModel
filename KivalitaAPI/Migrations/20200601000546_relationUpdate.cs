using Microsoft.EntityFrameworkCore.Migrations;

namespace KivalitaAPI.Migrations
{
    public partial class relationUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Role",
                table: "Filter");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Filter",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Sector",
                table: "Filter",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Filter");

            migrationBuilder.DropColumn(
                name: "Sector",
                table: "Filter");

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "Filter",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
