using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Uchu.Core.Migrations.MySql
{
    public partial class MySqlSocialUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Friends_Characters_FriendId",
                table: "Friends");

            migrationBuilder.DropForeignKey(
                name: "FK_Friends_Characters_FriendTwoId",
                table: "Friends");

            migrationBuilder.DropIndex(
                name: "IX_Friends_FriendId",
                table: "Friends");

            migrationBuilder.DropIndex(
                name: "IX_Friends_FriendTwoId",
                table: "Friends");

            migrationBuilder.DropColumn(
                name: "IsAccepted",
                table: "Friends");

            migrationBuilder.DropColumn(
                name: "IsBestFriend",
                table: "Friends");

            migrationBuilder.DropColumn(
                name: "IsDeclined",
                table: "Friends");

            migrationBuilder.DropColumn(
                name: "RequestHasBeenSent",
                table: "Friends");

            migrationBuilder.RenameColumn(
                name: "RequestingBestFriend",
                table: "Friends",
                newName: "BestFriend");

            migrationBuilder.RenameColumn(
                name: "FriendTwoId",
                table: "Friends",
                newName: "FriendB");

            migrationBuilder.RenameColumn(
                name: "FriendId",
                table: "Friends",
                newName: "FriendA");

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Friends",
                nullable: false,
                oldClrType: typeof(int))
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<int>(
                name: "LaunchedRocketFrom",
                table: "Characters",
                nullable: false,
                oldClrType: typeof(ushort));

            migrationBuilder.CreateTable(
                name: "ChatTranscript",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SentTime = table.Column<DateTime>(nullable: false),
                    Author = table.Column<long>(nullable: false),
                    Receiver = table.Column<long>(nullable: false),
                    Message = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatTranscript", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FriendRequests",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    BestFriend = table.Column<bool>(nullable: false),
                    Sender = table.Column<long>(nullable: false),
                    Receiver = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FriendRequests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Trades",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PartyA = table.Column<long>(nullable: false),
                    PartyB = table.Column<long>(nullable: false),
                    CurrencyA = table.Column<long>(nullable: false),
                    CurrencyB = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trades", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TransactionItems",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ItemId = table.Column<long>(nullable: false),
                    Party = table.Column<long>(nullable: false),
                    TradeId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TransactionItems_Trades_TradeId",
                        column: x => x.TradeId,
                        principalTable: "Trades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TransactionItems_TradeId",
                table: "TransactionItems",
                column: "TradeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChatTranscript");

            migrationBuilder.DropTable(
                name: "FriendRequests");

            migrationBuilder.DropTable(
                name: "TransactionItems");

            migrationBuilder.DropTable(
                name: "Trades");

            migrationBuilder.RenameColumn(
                name: "FriendB",
                table: "Friends",
                newName: "FriendTwoId");

            migrationBuilder.RenameColumn(
                name: "FriendA",
                table: "Friends",
                newName: "FriendId");

            migrationBuilder.RenameColumn(
                name: "BestFriend",
                table: "Friends",
                newName: "RequestingBestFriend");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Friends",
                nullable: false,
                oldClrType: typeof(long))
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<bool>(
                name: "IsAccepted",
                table: "Friends",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsBestFriend",
                table: "Friends",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeclined",
                table: "Friends",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "RequestHasBeenSent",
                table: "Friends",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<ushort>(
                name: "LaunchedRocketFrom",
                table: "Characters",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.CreateIndex(
                name: "IX_Friends_FriendId",
                table: "Friends",
                column: "FriendId");

            migrationBuilder.CreateIndex(
                name: "IX_Friends_FriendTwoId",
                table: "Friends",
                column: "FriendTwoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Friends_Characters_FriendId",
                table: "Friends",
                column: "FriendId",
                principalTable: "Characters",
                principalColumn: "CharacterId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Friends_Characters_FriendTwoId",
                table: "Friends",
                column: "FriendTwoId",
                principalTable: "Characters",
                principalColumn: "CharacterId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
