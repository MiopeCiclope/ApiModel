using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace KivalitaAPI.Migrations
{
    public partial class UpdateImage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ThumbnailData",
                table: "Image");

            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "Image",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Image",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileName",
                table: "Image");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Image");

            migrationBuilder.AddColumn<byte[]>(
                name: "ThumbnailData",
                table: "Image",
                type: "varbinary(max)",
                nullable: true);
        }
    }
}
