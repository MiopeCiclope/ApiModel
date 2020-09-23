using Microsoft.EntityFrameworkCore.Migrations;

namespace KivalitaAPI.Migrations
{
    public partial class ChangeFlowFilter : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<int>(
                name: "FlowId",
                table: "Filter",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Filter_FlowId",
                table: "Filter",
                column: "FlowId");

            migrationBuilder.AddForeignKey(
                name: "FK_Filter_Flow_FlowId",
                table: "Filter",
                column: "FlowId",
                principalTable: "Flow",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Filter_Flow_FlowId",
                table: "Filter");

            migrationBuilder.DropIndex(
                name: "IX_Filter_FlowId",
                table: "Filter");

            migrationBuilder.DropColumn(
                name: "FlowId",
                table: "Filter");

            migrationBuilder.AddColumn<int>(
                name: "FilterId",
                table: "FlowHistory",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FilterId",
                table: "Flow",
                type: "int",
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
    }
}
