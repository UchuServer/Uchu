using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Uchu.Core.Migrations
{
    [SuppressMessage("ReSharper", "CA1062")]
    public partial class PostgresSocialUpdate : Migration
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
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);

            migrationBuilder.CreateTable(
                name: "ChatTranscript",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
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
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
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
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
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
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
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
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);

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
