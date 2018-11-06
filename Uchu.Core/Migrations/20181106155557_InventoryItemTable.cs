using Microsoft.EntityFrameworkCore.Migrations;

namespace Uchu.Core.Migrations
{
    public partial class InventoryItemTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryItem_Characters_CharacterId",
                table: "InventoryItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InventoryItem",
                table: "InventoryItem");

            migrationBuilder.RenameTable(
                name: "InventoryItem",
                newName: "InventoryItems");

            migrationBuilder.RenameIndex(
                name: "IX_InventoryItem_CharacterId",
                table: "InventoryItems",
                newName: "IX_InventoryItems_CharacterId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InventoryItems",
                table: "InventoryItems",
                column: "InventoryItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryItems_Characters_CharacterId",
                table: "InventoryItems",
                column: "CharacterId",
                principalTable: "Characters",
                principalColumn: "CharacterId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryItems_Characters_CharacterId",
                table: "InventoryItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InventoryItems",
                table: "InventoryItems");

            migrationBuilder.RenameTable(
                name: "InventoryItems",
                newName: "InventoryItem");

            migrationBuilder.RenameIndex(
                name: "IX_InventoryItems_CharacterId",
                table: "InventoryItem",
                newName: "IX_InventoryItem_CharacterId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InventoryItem",
                table: "InventoryItem",
                column: "InventoryItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryItem_Characters_CharacterId",
                table: "InventoryItem",
                column: "CharacterId",
                principalTable: "Characters",
                principalColumn: "CharacterId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
