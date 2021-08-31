using Microsoft.EntityFrameworkCore.Migrations;

namespace Uchu.Core.Migrations.Sqlite
{
    public partial class SqliteLeaderboards : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ActivityScores",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Activity = table.Column<int>(type: "INTEGER", nullable: false),
                    Zone = table.Column<ushort>(type: "INTEGER", nullable: false),
                    CharacterId = table.Column<long>(type: "INTEGER", nullable: false),
                    Points = table.Column<int>(type: "INTEGER", nullable: false),
                    Time = table.Column<int>(type: "INTEGER", nullable: false),
                    LastPlayed = table.Column<long>(type: "INTEGER", nullable: false),
                    NumPlayed = table.Column<int>(type: "INTEGER", nullable: false),
                    Week = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityScores", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActivityScores");
        }
    }
}
