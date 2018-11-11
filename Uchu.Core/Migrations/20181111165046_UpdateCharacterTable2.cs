using Microsoft.EntityFrameworkCore.Migrations;

namespace Uchu.Core.Migrations
{
    public partial class UpdateCharacterTable2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LegoScore",
                table: "Characters",
                newName: "UniverseScore");

            migrationBuilder.AddColumn<long>(
                name: "Currency",
                table: "Characters",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<int>(
                name: "CurrentArmor",
                table: "Characters",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CurrentHealth",
                table: "Characters",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CurrentImagination",
                table: "Characters",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaximumArmor",
                table: "Characters",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaximumHealth",
                table: "Characters",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaximumImagination",
                table: "Characters",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Currency",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "CurrentArmor",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "CurrentHealth",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "CurrentImagination",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "MaximumArmor",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "MaximumHealth",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "MaximumImagination",
                table: "Characters");

            migrationBuilder.RenameColumn(
                name: "UniverseScore",
                table: "Characters",
                newName: "LegoScore");
        }
    }
}
