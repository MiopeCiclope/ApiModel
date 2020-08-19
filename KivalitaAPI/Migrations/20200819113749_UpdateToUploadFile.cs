using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace KivalitaAPI.Migrations
{
    public partial class UpdateToUploadFile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageData",
                table: "Image");

            migrationBuilder.DropColumn(
                name: "ThumbnailData",
                table: "Image");

            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "Image",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileName",
                table: "Image");

            migrationBuilder.AddColumn<byte[]>(
                name: "ImageData",
                table: "Image",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "ThumbnailData",
                table: "Image",
                type: "varbinary(max)",
                nullable: true);
        }
    }
}
