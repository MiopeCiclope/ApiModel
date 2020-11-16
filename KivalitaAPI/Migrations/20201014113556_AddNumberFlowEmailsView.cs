using Microsoft.EntityFrameworkCore.Migrations;

namespace KivalitaAPI.Migrations
{
    public partial class AddNumberFlowEmailsView : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE VIEW NumberFlowEmails as
	                SELECT flowid,
                    Count(*) AS TotalSent,
                    Sum(CASE
                    WHEN mailid IS NULL THEN 0
                    ELSE 1
                    END) AS MailAnswered
                    FROM (SELECT f.id AS flowId,
                    ma.id AS mailid
                    FROM flow f
                    INNER JOIN (select id, flowid, type from flowAction where type = 'email') fa
                    ON fa.flowid = f.id
                    INNER JOIN (select id, status, flowactionid from flowtask where status = 'finished') ft
                    ON ft.flowactionid = fa.id
                    LEFT OUTER JOIN mailanswered ma
                    ON ma.taskid = ft.id AND ma.Status != 3 ) AS task
                    GROUP BY flowid
                go
            ");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW dbo.NumberFlowEmails");
        }
    }
}
