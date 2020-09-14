using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace KivalitaAPI.Migrations
{
    public partial class AddPreLead : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PreLead",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(nullable: true),
                    CreatedBy = table.Column<int>(nullable: false),
                    UpdatedBy = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PreLead", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PreLeadHistory",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(nullable: true),
                    TableId = table.Column<int>(nullable: false),
                    Action = table.Column<int>(nullable: false),
                    Responsable = table.Column<int>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PreLeadHistory", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PreLead");

            migrationBuilder.DropTable(
                name: "PreLeadHistory");
        }
    }
}
