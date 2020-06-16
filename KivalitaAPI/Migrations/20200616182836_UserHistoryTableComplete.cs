using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace KivalitaAPI.Migrations
{
    public partial class UserHistoryTableComplete : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Action",
                table: "UserHistory",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "UserHistory",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "UserHistory",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Responsable",
                table: "UserHistory",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Action",
                table: "UserHistory");

            migrationBuilder.DropColumn(
                name: "Date",
                table: "UserHistory");

            migrationBuilder.DropColumn(
                name: "Password",
                table: "UserHistory");

            migrationBuilder.DropColumn(
                name: "Responsable",
                table: "UserHistory");
        }
    }
}
