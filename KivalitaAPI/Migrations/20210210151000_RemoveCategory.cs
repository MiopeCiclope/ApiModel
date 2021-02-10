using Microsoft.EntityFrameworkCore.Migrations;

namespace KivalitaAPI.Migrations
{
    public partial class RemoveCategory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Template_Category_CategoryId",
                table: "Template");

            migrationBuilder.DropIndex(
                name: "IX_Template_CategoryId",
                table: "Template");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "TemplateHistory");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Template");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "TemplateHistory",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "Template",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Template_CategoryId",
                table: "Template",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Template_Category_CategoryId",
                table: "Template",
                column: "CategoryId",
                principalTable: "Category",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
