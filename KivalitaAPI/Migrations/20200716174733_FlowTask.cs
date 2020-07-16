using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace KivalitaAPI.Migrations
{
    public partial class FlowTask : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FlowTask",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Status = table.Column<string>(nullable: true),
                    ScheduledTo = table.Column<DateTime>(nullable: true),
                    LeadId = table.Column<int>(nullable: false),
                    FlowActionId = table.Column<int>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    UpdatedBy = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlowTask", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FlowTask_FlowAction_FlowActionId",
                        column: x => x.FlowActionId,
                        principalTable: "FlowAction",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FlowTask_Leads_LeadId",
                        column: x => x.LeadId,
                        principalTable: "Leads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FlowTaskHistory",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Status = table.Column<string>(nullable: true),
                    ScheduledTo = table.Column<DateTime>(nullable: false),
                    LeadId = table.Column<int>(nullable: false),
                    FlowActionId = table.Column<int>(nullable: false),
                    TableId = table.Column<int>(nullable: false),
                    Action = table.Column<int>(nullable: false),
                    Responsable = table.Column<int>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlowTaskHistory", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FlowTask_FlowActionId",
                table: "FlowTask",
                column: "FlowActionId");

            migrationBuilder.CreateIndex(
                name: "IX_FlowTask_LeadId",
                table: "FlowTask",
                column: "LeadId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FlowTask");

            migrationBuilder.DropTable(
                name: "FlowTaskHistory");
        }
    }
}
