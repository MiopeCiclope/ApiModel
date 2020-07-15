using Microsoft.EntityFrameworkCore.Migrations;

namespace KivalitaAPI.Migrations
{
    public partial class FKFlowFilter : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FilterId",
                table: "FlowHistory",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FilterId",
                table: "Flow",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Flow_FilterId",
                table: "Flow",
                column: "FilterId");

            migrationBuilder.AddForeignKey(
                name: "FK_Flow_Filter_FilterId",
                table: "Flow",
                column: "FilterId",
                principalTable: "Filter",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Flow_Filter_FilterId",
                table: "Flow");

            migrationBuilder.DropIndex(
                name: "IX_Flow_FilterId",
                table: "Flow");

            migrationBuilder.DropColumn(
                name: "FilterId",
                table: "FlowHistory");

            migrationBuilder.DropColumn(
                name: "FilterId",
                table: "Flow");
        }
    }
}
