using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Uchu.Core.Migrations
{
    [SuppressMessage("ReSharper", "CA1062")]
    public partial class PostgresFixedMail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Mails_Characters_AuthorId",
                table: "Mails");

            migrationBuilder.DropForeignKey(
                name: "FK_Mails_Characters_RecipientId",
                table: "Mails");

            migrationBuilder.DropIndex(
                name: "IX_Mails_AuthorId",
                table: "Mails");

            migrationBuilder.DropIndex(
                name: "IX_Mails_RecipientId",
                table: "Mails");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Mails_AuthorId",
                table: "Mails",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Mails_RecipientId",
                table: "Mails",
                column: "RecipientId");

            migrationBuilder.AddForeignKey(
                name: "FK_Mails_Characters_AuthorId",
                table: "Mails",
                column: "AuthorId",
                principalTable: "Characters",
                principalColumn: "CharacterId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Mails_Characters_RecipientId",
                table: "Mails",
                column: "RecipientId",
                principalTable: "Characters",
                principalColumn: "CharacterId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
