using Microsoft.EntityFrameworkCore.Migrations;

namespace Uchu.Core.Migrations
{
    public partial class LaunchedFromProperty : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LaunchedRocketFrom",
                table: "Characters",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LaunchedRocketFrom",
                table: "Characters");
        }
    }
}
