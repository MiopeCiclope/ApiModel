using Microsoft.EntityFrameworkCore.Migrations;

namespace KivalitaAPI.Migrations
{
    public partial class UpdateMailAnswered : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "MailAnswered",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MailAnswered_LeadId",
                table: "MailAnswered",
                column: "LeadId");

            migrationBuilder.CreateIndex(
                name: "IX_MailAnswered_TaskId",
                table: "MailAnswered",
                column: "TaskId");

            migrationBuilder.AddForeignKey(
                name: "FK_MailAnswered_Leads_LeadId",
                table: "MailAnswered",
                column: "LeadId",
                principalTable: "Leads",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_MailAnswered_FlowTask_TaskId",
                table: "MailAnswered",
                column: "TaskId",
                principalTable: "FlowTask",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MailAnswered_Leads_LeadId",
                table: "MailAnswered");

            migrationBuilder.DropForeignKey(
                name: "FK_MailAnswered_FlowTask_TaskId",
                table: "MailAnswered");

            migrationBuilder.DropIndex(
                name: "IX_MailAnswered_LeadId",
                table: "MailAnswered");

            migrationBuilder.DropIndex(
                name: "IX_MailAnswered_TaskId",
                table: "MailAnswered");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "MailAnswered");
        }
    }
}
