using Microsoft.EntityFrameworkCore.Migrations;

namespace KivalitaAPI.Migrations
{
    public partial class Owners : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Owner",
                table: "Template",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Owner",
                table: "Flow",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Template_Owner",
                table: "Template",
                column: "Owner");

            migrationBuilder.CreateIndex(
                name: "IX_Flow_Owner",
                table: "Flow",
                column: "Owner");

            migrationBuilder.AddForeignKey(
                name: "FK_Flow_User_Owner",
                table: "Flow",
                column: "Owner",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Template_User_Owner",
                table: "Template",
                column: "Owner",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Flow_User_Owner",
                table: "Flow");

            migrationBuilder.DropForeignKey(
                name: "FK_Template_User_Owner",
                table: "Template");

            migrationBuilder.DropIndex(
                name: "IX_Template_Owner",
                table: "Template");

            migrationBuilder.DropIndex(
                name: "IX_Flow_Owner",
                table: "Flow");

            migrationBuilder.DropColumn(
                name: "Owner",
                table: "Template");

            migrationBuilder.DropColumn(
                name: "Owner",
                table: "Flow");
        }
    }
}
