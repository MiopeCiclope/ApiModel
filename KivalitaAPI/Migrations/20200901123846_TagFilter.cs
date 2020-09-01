using Microsoft.EntityFrameworkCore.Migrations;

namespace KivalitaAPI.Migrations
{
    public partial class TagFilter : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TagId",
                table: "Filter",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Filter_TagId",
                table: "Filter",
                column: "TagId");

            migrationBuilder.AddForeignKey(
                name: "FK_Filter_Tag_TagId",
                table: "Filter",
                column: "TagId",
                principalTable: "Tag",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Filter_Tag_TagId",
                table: "Filter");

            migrationBuilder.DropIndex(
                name: "IX_Filter_TagId",
                table: "Filter");

            migrationBuilder.DropColumn(
                name: "TagId",
                table: "Filter");
        }
    }
}
