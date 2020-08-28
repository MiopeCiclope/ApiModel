using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace KivalitaAPI.Migrations
{
    public partial class MailSignature : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Signature",
                table: "User");

            migrationBuilder.AddColumn<int>(
                name: "MailSignatureId",
                table: "UserHistory",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MailSignatureId",
                table: "User",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MailSignature",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(nullable: false),
                    Signature = table.Column<string>(nullable: true),
                    CreatedBy = table.Column<int>(nullable: false),
                    UpdatedBy = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MailSignature", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MailSignature_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MailSignature_UserId",
                table: "MailSignature",
                column: "UserId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MailSignature");

            migrationBuilder.DropColumn(
                name: "MailSignatureId",
                table: "UserHistory");

            migrationBuilder.DropColumn(
                name: "MailSignatureId",
                table: "User");

            migrationBuilder.AddColumn<string>(
                name: "Signature",
                table: "User",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
