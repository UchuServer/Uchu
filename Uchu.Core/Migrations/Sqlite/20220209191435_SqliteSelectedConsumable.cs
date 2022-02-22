using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Uchu.Core.Migrations.Sqlite
{
    public partial class SqliteSelectedConsumable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SelectedConsumable",
                table: "Characters",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SelectedConsumable",
                table: "Characters");
        }
    }
}
