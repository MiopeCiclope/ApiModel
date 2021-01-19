using Microsoft.EntityFrameworkCore.Migrations;

namespace KivalitaAPI.Migrations
{
    public partial class CompanyState : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "CompanyHistory",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "CompanyHistory",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Company",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "Company",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "City",
                table: "CompanyHistory");

            migrationBuilder.DropColumn(
                name: "State",
                table: "CompanyHistory");

            migrationBuilder.DropColumn(
                name: "City",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "State",
                table: "Company");
        }
    }
}
