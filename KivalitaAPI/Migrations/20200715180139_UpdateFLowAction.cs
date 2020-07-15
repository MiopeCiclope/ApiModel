using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace KivalitaAPI.Migrations
{
    public partial class UpdateFLowAction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Date",
                table: "FlowAction");

            migrationBuilder.AddColumn<int>(
                name: "afterDays",
                table: "FlowActionHistory",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "afterDays",
                table: "FlowAction",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "afterDays",
                table: "FlowActionHistory");

            migrationBuilder.DropColumn(
                name: "afterDays",
                table: "FlowAction");

            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "FlowAction",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
