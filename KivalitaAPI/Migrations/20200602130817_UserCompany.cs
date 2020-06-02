using Microsoft.EntityFrameworkCore.Migrations;

namespace KivalitaAPI.Migrations
{
    public partial class UserCompany : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Leads_Company_CompanyId",
                table: "Leads");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Company",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Company_UserId",
                table: "Company",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Company_User_UserId",
                table: "Company",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

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
                name: "FK_Company_User_UserId",
                table: "Company");

            migrationBuilder.DropForeignKey(
                name: "FK_Leads_Company_CompanyId",
                table: "Leads");

            migrationBuilder.DropIndex(
                name: "IX_Company_UserId",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Company");

            migrationBuilder.AddForeignKey(
                name: "FK_Leads_Company_CompanyId",
                table: "Leads",
                column: "CompanyId",
                principalTable: "Company",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
