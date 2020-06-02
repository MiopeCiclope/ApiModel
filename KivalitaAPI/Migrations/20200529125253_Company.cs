using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace KivalitaAPI.Migrations
{
    public partial class Company : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Company",
                table: "Leads");

            migrationBuilder.DropColumn(
                name: "CompanyLinkedIn",
                table: "Leads");

            migrationBuilder.DropColumn(
                name: "CompanySite",
                table: "Leads");

            migrationBuilder.DropColumn(
                name: "Sector",
                table: "Leads");

            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                table: "Leads",
                nullable: true,
                defaultValue: null);

            migrationBuilder.CreateTable(
                name: "Company",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    Sector = table.Column<string>(nullable: true),
                    Site = table.Column<string>(nullable: true),
                    LinkedIn = table.Column<string>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    UpdatedBy = table.Column<int>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Company", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Leads_CompanyId",
                table: "Leads",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Leads_Company_CompanyId",
                table: "Leads",
                column: "CompanyId",
                principalTable: "Company",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Leads_Company_CompanyId",
                table: "Leads");

            migrationBuilder.DropTable(
                name: "Company");

            migrationBuilder.DropIndex(
                name: "IX_Leads_CompanyId",
                table: "Leads");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "Leads");

            migrationBuilder.AddColumn<string>(
                name: "Company",
                table: "Leads",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CompanyLinkedIn",
                table: "Leads",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CompanySite",
                table: "Leads",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Sector",
                table: "Leads",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
