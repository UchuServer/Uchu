using Microsoft.EntityFrameworkCore.Migrations;

namespace Uchu.Core.Migrations
{
    public partial class UpdateMissionTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Completed",
                table: "Missions");

            migrationBuilder.AddColumn<int>(
                name: "State",
                table: "Missions",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "State",
                table: "Missions");

            migrationBuilder.AddColumn<bool>(
                name: "Completed",
                table: "Missions",
                nullable: false,
                defaultValue: false);
        }
    }
}
