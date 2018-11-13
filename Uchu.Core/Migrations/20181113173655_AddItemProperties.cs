using Microsoft.EntityFrameworkCore.Migrations;

namespace Uchu.Core.Migrations
{
    public partial class AddItemProperties : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsBound",
                table: "InventoryItems",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsEquipped",
                table: "InventoryItems",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsBound",
                table: "InventoryItems");

            migrationBuilder.DropColumn(
                name: "IsEquipped",
                table: "InventoryItems");
        }
    }
}
