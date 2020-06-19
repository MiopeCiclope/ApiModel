using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace KivalitaAPI.Migrations
{
    public partial class AddThumbnailField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "ThumbnailData",
                table: "ImageHistory",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "ThumbnailData",
                table: "Image",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ThumbnailData",
                table: "ImageHistory");

            migrationBuilder.DropColumn(
                name: "ThumbnailData",
                table: "Image");
        }
    }
}
