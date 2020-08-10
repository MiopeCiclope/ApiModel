using Microsoft.EntityFrameworkCore.Migrations;

namespace KivalitaAPI.Migrations
{
    public partial class AddFlowIdOnLead : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FlowId",
                table: "LeadsHistory",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FlowId",
                table: "Leads",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Leads_FlowId",
                table: "Leads",
                column: "FlowId");

            migrationBuilder.AddForeignKey(
                name: "FK_Leads_Flow_FlowId",
                table: "Leads",
                column: "FlowId",
                principalTable: "Flow",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Leads_Flow_FlowId",
                table: "Leads");

            migrationBuilder.DropIndex(
                name: "IX_Leads_FlowId",
                table: "Leads");

            migrationBuilder.DropColumn(
                name: "FlowId",
                table: "LeadsHistory");

            migrationBuilder.DropColumn(
                name: "FlowId",
                table: "Leads");
        }
    }
}
