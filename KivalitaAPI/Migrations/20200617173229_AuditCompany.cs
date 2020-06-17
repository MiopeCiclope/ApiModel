using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace KivalitaAPI.Migrations
{
    public partial class AuditCompany : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CompanyHistory",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    Sector = table.Column<string>(nullable: true),
                    Site = table.Column<string>(nullable: true),
                    LinkedIn = table.Column<string>(nullable: true),
                    UserId = table.Column<int>(nullable: true),
                    TableId = table.Column<int>(nullable: false),
                    Action = table.Column<int>(nullable: false),
                    Responsable = table.Column<int>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyHistory", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CompanyHistory");
        }
    }
}
