using Microsoft.EntityFrameworkCore.Migrations;

namespace Uchu.Core.Migrations.MySql
{
    public partial class MySqlAddSpawnPersistence : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SpawnLocationName",
                table: "Characters",
                type: "varchar(33)",
                maxLength: 33,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<float>(
                name: "SpawnPositionX",
                table: "Characters",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "SpawnPositionY",
                table: "Characters",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "SpawnPositionZ",
                table: "Characters",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "SpawnRotationW",
                table: "Characters",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "SpawnRotationX",
                table: "Characters",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "SpawnRotationY",
                table: "Characters",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "SpawnRotationZ",
                table: "Characters",
                type: "float",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SpawnLocationName",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "SpawnPositionX",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "SpawnPositionY",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "SpawnPositionZ",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "SpawnRotationW",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "SpawnRotationX",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "SpawnRotationY",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "SpawnRotationZ",
                table: "Characters");
        }
    }
}
