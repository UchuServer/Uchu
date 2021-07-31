using Microsoft.EntityFrameworkCore.Migrations;

namespace Uchu.Core.Migrations.CdClient
{
    public partial class GoToNpcCorrections : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Set the GoToNPC tasks targets.
            migrationBuilder?.Sql("UPDATE MissionTasks SET target = 7414 WHERE id = 734;");
            migrationBuilder?.Sql("UPDATE MissionTasks SET target = 13789 WHERE id = 1797;");
            migrationBuilder?.Sql("UPDATE MissionTasks SET target = 13792 WHERE id = 1798;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Revert the GoToNPC tasks targets.
            migrationBuilder?.Sql("UPDATE MissionTasks SET target = 3036 WHERE id = 734;");
            migrationBuilder?.Sql("UPDATE MissionTasks SET target = 13879 WHERE id = 1797;");
            migrationBuilder?.Sql("UPDATE MissionTasks SET target = 13798 WHERE id = 1798;");
        }
    }
}
