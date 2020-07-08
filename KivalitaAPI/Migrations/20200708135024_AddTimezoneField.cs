using Microsoft.EntityFrameworkCore.Migrations;

namespace KivalitaAPI.Migrations
{
    public partial class AddTimezoneField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Timezone",
                table: "User",
                defaultValue: "America/Sao_Paulo",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Timezone",
                table: "User");
        }
    }
}
