using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace KivalitaAPI.Migrations
{
    public partial class AddTaskNote : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TaskNote",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    description = table.Column<string>(nullable: true),
                    FlowTaskId = table.Column<int>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    UpdatedBy = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskNote", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskNote_FlowTask_FlowTaskId",
                        column: x => x.FlowTaskId,
                        principalTable: "FlowTask",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TaskNoteHistory",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    description = table.Column<string>(nullable: true),
                    FlowTaskId = table.Column<int>(nullable: false),
                    TableId = table.Column<int>(nullable: false),
                    Action = table.Column<int>(nullable: false),
                    Responsable = table.Column<int>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskNoteHistory", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TaskNote_FlowTaskId",
                table: "TaskNote",
                column: "FlowTaskId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TaskNote");

            migrationBuilder.DropTable(
                name: "TaskNoteHistory");
        }
    }
}
