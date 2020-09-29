using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace KivalitaAPI.Migrations
{
    public partial class MailAnswered : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MailAnswered",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(nullable: false),
                    LeadId = table.Column<int>(nullable: false),
                    TaskId = table.Column<int>(nullable: false),
                    MessageId = table.Column<string>(nullable: true),
                    Subject = table.Column<string>(nullable: true),
                    BodyPreview = table.Column<string>(nullable: true),
                    BodyContent = table.Column<string>(nullable: true),
                    Sender = table.Column<string>(nullable: true),
                    Recipient = table.Column<string>(nullable: true),
                    CreatedBy = table.Column<int>(nullable: false),
                    UpdatedBy = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MailAnswered", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MailAnsweredHistory",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LeadId = table.Column<int>(nullable: false),
                    TaskId = table.Column<int>(nullable: false),
                    MessageId = table.Column<string>(nullable: true),
                    Subject = table.Column<string>(nullable: true),
                    BodyPreview = table.Column<string>(nullable: true),
                    BodyContent = table.Column<string>(nullable: true),
                    Sender = table.Column<string>(nullable: true),
                    Recipient = table.Column<string>(nullable: true),
                    TableId = table.Column<int>(nullable: false),
                    Action = table.Column<int>(nullable: false),
                    Responsable = table.Column<int>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MailAnsweredHistory", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MailAnswered");

            migrationBuilder.DropTable(
                name: "MailAnsweredHistory");
        }
    }
}
