using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace KivalitaAPI.Migrations
{
    public partial class Fluxo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Flow",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    actionForAllLeads = table.Column<bool>(nullable: false),
                    leadGroupSize = table.Column<int>(nullable: false),
                    isAutomatic = table.Column<bool>(nullable: false),
                    DaysOfTheWeek = table.Column<string>(nullable: true),
                    TagAsLost = table.Column<bool>(nullable: false),
                    EndLead = table.Column<bool>(nullable: false),
                    SendMoskit = table.Column<bool>(nullable: false),
                    SendRdStation = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    UpdatedBy = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Flow", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FlowHistory",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    actionForAllLeads = table.Column<bool>(nullable: false),
                    leadGroupSize = table.Column<int>(nullable: false),
                    isAutomatic = table.Column<bool>(nullable: false),
                    DaysOfTheWeek = table.Column<string>(nullable: true),
                    TagAsLost = table.Column<bool>(nullable: false),
                    EndLead = table.Column<bool>(nullable: false),
                    SendMoskit = table.Column<bool>(nullable: false),
                    SendRdStation = table.Column<bool>(nullable: false),
                    TableId = table.Column<int>(nullable: false),
                    Action = table.Column<int>(nullable: false),
                    Responsable = table.Column<int>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlowHistory", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Flow");

            migrationBuilder.DropTable(
                name: "FlowHistory");
        }
    }
}
