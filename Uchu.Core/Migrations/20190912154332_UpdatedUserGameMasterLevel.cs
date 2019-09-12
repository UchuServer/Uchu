using Microsoft.EntityFrameworkCore.Migrations;

namespace Uchu.Core.Migrations
{
    public partial class UpdatedUserGameMasterLevel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GameMasterLevel",
                table: "Users",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GameMasterLevel",
                table: "Users");
        }
    }
}
