using Microsoft.EntityFrameworkCore.Migrations;

namespace Uchu.Core.Migrations.MySql
{
    public partial class MySqlRefactor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Users",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "LOT",
                table: "InventoryItems",
                newName: "Lot");

            migrationBuilder.RenameColumn(
                name: "InventoryItemId",
                table: "InventoryItems",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "CharacterId",
                table: "Characters",
                newName: "Id");

            migrationBuilder.AddColumn<long>(
                name: "ParentId",
                table: "InventoryItems",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<bool>(
                name: "Sent",
                table: "FriendRequests",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "InventoryItems");

            migrationBuilder.DropColumn(
                name: "Sent",
                table: "FriendRequests");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Users",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "Lot",
                table: "InventoryItems",
                newName: "LOT");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "InventoryItems",
                newName: "InventoryItemId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Characters",
                newName: "CharacterId");
        }
    }
}
