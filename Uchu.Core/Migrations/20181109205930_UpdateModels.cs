using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Uchu.Core.Migrations
{
    public partial class UpdateModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "LOT",
                table: "InventoryItems",
                nullable: false,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<long>(
                name: "InventoryItemId",
                table: "InventoryItems",
                nullable: false,
                oldClrType: typeof(int))
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);

            migrationBuilder.AddColumn<long>(
                name: "Count",
                table: "InventoryItems",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<int>(
                name: "Slot",
                table: "InventoryItems",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<long>(
                name: "LegoScore",
                table: "Characters",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "Level",
                table: "Characters",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "TotalArmorPowerUpsCollected",
                table: "Characters",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "TotalArmorRepaired",
                table: "Characters",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "TotalBricksCollected",
                table: "Characters",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "TotalCurrencyCollected",
                table: "Characters",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "TotalDamageHealed",
                table: "Characters",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "TotalDamageTaken",
                table: "Characters",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "TotalDistanceDriven",
                table: "Characters",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "TotalDistanceTraveled",
                table: "Characters",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "TotalEnemiesSmashed",
                table: "Characters",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "TotalFirstPlaceFinishes",
                table: "Characters",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "TotalImaginationPowerUpsCollected",
                table: "Characters",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "TotalImaginationRestored",
                table: "Characters",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "TotalImaginationUsed",
                table: "Characters",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "TotalLifePowerUpsCollected",
                table: "Characters",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "TotalMissionsCompleted",
                table: "Characters",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "TotalPetsTamed",
                table: "Characters",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "TotalQuickBuildsCompleted",
                table: "Characters",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "TotalRacecarBoostsActivated",
                table: "Characters",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "TotalRacecarWrecks",
                table: "Characters",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "TotalRacesFinished",
                table: "Characters",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "TotalRacingImaginationCratesSmashed",
                table: "Characters",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "TotalRacingImaginationPowerUpsCollected",
                table: "Characters",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "TotalRacingSmashablesSmashed",
                table: "Characters",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "TotalRocketsUsed",
                table: "Characters",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "TotalSmashablesSmashed",
                table: "Characters",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "TotalSuicides",
                table: "Characters",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "TotalTimeAirborne",
                table: "Characters",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Count",
                table: "InventoryItems");

            migrationBuilder.DropColumn(
                name: "Slot",
                table: "InventoryItems");

            migrationBuilder.DropColumn(
                name: "LegoScore",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "Level",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "TotalArmorPowerUpsCollected",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "TotalArmorRepaired",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "TotalBricksCollected",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "TotalCurrencyCollected",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "TotalDamageHealed",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "TotalDamageTaken",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "TotalDistanceDriven",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "TotalDistanceTraveled",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "TotalEnemiesSmashed",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "TotalFirstPlaceFinishes",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "TotalImaginationPowerUpsCollected",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "TotalImaginationRestored",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "TotalImaginationUsed",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "TotalLifePowerUpsCollected",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "TotalMissionsCompleted",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "TotalPetsTamed",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "TotalQuickBuildsCompleted",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "TotalRacecarBoostsActivated",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "TotalRacecarWrecks",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "TotalRacesFinished",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "TotalRacingImaginationCratesSmashed",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "TotalRacingImaginationPowerUpsCollected",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "TotalRacingSmashablesSmashed",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "TotalRocketsUsed",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "TotalSmashablesSmashed",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "TotalSuicides",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "TotalTimeAirborne",
                table: "Characters");

            migrationBuilder.AlterColumn<long>(
                name: "LOT",
                table: "InventoryItems",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "InventoryItemId",
                table: "InventoryItems",
                nullable: false,
                oldClrType: typeof(long))
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);
        }
    }
}
