using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Uchu.Core.Migrations
{
    public partial class UpdateCharacterTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CustomName",
                table: "Characters",
                maxLength: 33,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "EyeStyle",
                table: "Characters",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "EyebrowStyle",
                table: "Characters",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<bool>(
                name: "FreeToPlay",
                table: "Characters",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "HairColor",
                table: "Characters",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "HairStyle",
                table: "Characters",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "LastActivity",
                table: "Characters",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "LastClone",
                table: "Characters",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<int>(
                name: "LastInstance",
                table: "Characters",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastZone",
                table: "Characters",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<long>(
                name: "Lh",
                table: "Characters",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "MouthStyle",
                table: "Characters",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<bool>(
                name: "NameRejected",
                table: "Characters",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "PantsColor",
                table: "Characters",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "Rh",
                table: "Characters",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "ShirtColor",
                table: "Characters",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "ShirtStyle",
                table: "Characters",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "InventoryItem",
                columns: table => new
                {
                    InventoryItemId = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    LOT = table.Column<long>(nullable: false),
                    CharacterId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryItem", x => x.InventoryItemId);
                    table.ForeignKey(
                        name: "FK_InventoryItem_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "CharacterId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryItem_CharacterId",
                table: "InventoryItem",
                column: "CharacterId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InventoryItem");

            migrationBuilder.DropColumn(
                name: "CustomName",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "EyeStyle",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "EyebrowStyle",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "FreeToPlay",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "HairColor",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "HairStyle",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "LastActivity",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "LastClone",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "LastInstance",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "LastZone",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "Lh",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "MouthStyle",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "NameRejected",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "PantsColor",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "Rh",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "ShirtColor",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "ShirtStyle",
                table: "Characters");
        }
    }
}
