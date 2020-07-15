using Microsoft.EntityFrameworkCore.Migrations;

namespace KivalitaAPI.Migrations
{
    public partial class UpdateFieldsFlow : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndLead",
                table: "FlowHistory");

            migrationBuilder.DropColumn(
                name: "SendMoskit",
                table: "FlowHistory");

            migrationBuilder.DropColumn(
                name: "SendRdStation",
                table: "FlowHistory");

            migrationBuilder.DropColumn(
                name: "TagAsLost",
                table: "FlowHistory");

            migrationBuilder.DropColumn(
                name: "EndLead",
                table: "Flow");

            migrationBuilder.DropColumn(
                name: "SendMoskit",
                table: "Flow");

            migrationBuilder.DropColumn(
                name: "SendRdStation",
                table: "Flow");

            migrationBuilder.DropColumn(
                name: "TagAsLost",
                table: "Flow");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "FlowActionHistory",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "FlowId",
                table: "FlowAction",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "FlowAction",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "FlowActionHistory");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "FlowAction");

            migrationBuilder.AddColumn<bool>(
                name: "EndLead",
                table: "FlowHistory",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SendMoskit",
                table: "FlowHistory",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SendRdStation",
                table: "FlowHistory",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "TagAsLost",
                table: "FlowHistory",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<int>(
                name: "FlowId",
                table: "FlowAction",
                type: "int",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<bool>(
                name: "EndLead",
                table: "Flow",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SendMoskit",
                table: "Flow",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SendRdStation",
                table: "Flow",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "TagAsLost",
                table: "Flow",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
