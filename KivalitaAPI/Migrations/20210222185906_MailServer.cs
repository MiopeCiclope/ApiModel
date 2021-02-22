using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace KivalitaAPI.Migrations
{
    public partial class MailServer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MailCredentialId",
                table: "UserHistory",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MailCredentialId",
                table: "User",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MailCredential",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    email = table.Column<string>(nullable: true),
                    password = table.Column<string>(nullable: true),
                    MailServerId = table.Column<int>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    UpdatedBy = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MailCredential", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MailCredentialHistory",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    email = table.Column<string>(nullable: true),
                    password = table.Column<string>(nullable: true),
                    MailServerId = table.Column<int>(nullable: false),
                    TableId = table.Column<int>(nullable: false),
                    Action = table.Column<int>(nullable: false),
                    Responsable = table.Column<int>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MailCredentialHistory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MailServer",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    SMTP = table.Column<string>(nullable: true),
                    POP = table.Column<string>(nullable: true),
                    IMAP = table.Column<string>(nullable: true),
                    useSSL = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    UpdatedBy = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MailServer", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MailServerHistory",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    SMTP = table.Column<string>(nullable: true),
                    POP = table.Column<string>(nullable: true),
                    IMAP = table.Column<string>(nullable: true),
                    useSSL = table.Column<bool>(nullable: false),
                    TableId = table.Column<int>(nullable: false),
                    Action = table.Column<int>(nullable: false),
                    Responsable = table.Column<int>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MailServerHistory", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_User_MailCredentialId",
                table: "User",
                column: "MailCredentialId");

            migrationBuilder.AddForeignKey(
                name: "FK_User_MailCredential_MailCredentialId",
                table: "User",
                column: "MailCredentialId",
                principalTable: "MailCredential",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_User_MailCredential_MailCredentialId",
                table: "User");

            migrationBuilder.DropTable(
                name: "MailCredential");

            migrationBuilder.DropTable(
                name: "MailCredentialHistory");

            migrationBuilder.DropTable(
                name: "MailServer");

            migrationBuilder.DropTable(
                name: "MailServerHistory");

            migrationBuilder.DropIndex(
                name: "IX_User_MailCredentialId",
                table: "User");

            migrationBuilder.DropColumn(
                name: "MailCredentialId",
                table: "UserHistory");

            migrationBuilder.DropColumn(
                name: "MailCredentialId",
                table: "User");
        }
    }
}
