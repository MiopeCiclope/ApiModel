using Microsoft.EntityFrameworkCore.Migrations;

namespace KivalitaAPI.Migrations
{
    public partial class NonTableMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Filter",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            //migrationBuilder.CreateTable(
            //    name: "TaskDTO",
            //    columns: table => new
            //    {
            //        TaskId = table.Column<int>(nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        UserId = table.Column<int>(nullable: false),
            //        LeadId = table.Column<int>(nullable: false),
            //        Email = table.Column<string>(nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_TaskDTO", x => x.TaskId);
            //    });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropTable(
            //    name: "TaskDTO");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Filter",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);
        }
    }
}
