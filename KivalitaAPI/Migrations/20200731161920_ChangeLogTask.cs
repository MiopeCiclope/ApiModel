using Microsoft.EntityFrameworkCore.Migrations;

namespace KivalitaAPI.Migrations
{
    public partial class ChangeLogTask : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LeadId",
                table: "LogTasks",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_LogTasks_LeadId",
                table: "LogTasks",
                column: "LeadId");

            migrationBuilder.AddForeignKey(
                name: "FK_LogTasks_Leads_LeadId",
                table: "LogTasks",
                column: "LeadId",
                principalTable: "Leads",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LogTasks_Leads_LeadId",
                table: "LogTasks");

            migrationBuilder.DropIndex(
                name: "IX_LogTasks_LeadId",
                table: "LogTasks");

            migrationBuilder.DropColumn(
                name: "LeadId",
                table: "LogTasks");
        }
    }
}
