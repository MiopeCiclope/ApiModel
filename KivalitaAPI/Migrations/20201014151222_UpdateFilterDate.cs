using Microsoft.EntityFrameworkCore.Migrations;

namespace KivalitaAPI.Migrations
{
    public partial class UpdateFilterDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Date",
                table: "Filter");

            migrationBuilder.AddColumn<string>(
                name: "EndDate",
                table: "Filter",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StartDate",
                table: "Filter",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "Filter");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "Filter");

            migrationBuilder.AddColumn<string>(
                name: "Date",
                table: "Filter",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
