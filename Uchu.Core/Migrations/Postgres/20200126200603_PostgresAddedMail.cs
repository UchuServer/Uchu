using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Uchu.Core.Migrations
{
    public partial class PostgresAddedMail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Mails",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Subject = table.Column<string>(nullable: true),
                    Body = table.Column<string>(nullable: true),
                    AttachmentLot = table.Column<int>(nullable: false),
                    AttachmentCount = table.Column<int>(nullable: false),
                    AttachmentCurrency = table.Column<decimal>(nullable: false),
                    ExpirationTime = table.Column<DateTime>(nullable: false),
                    SentTime = table.Column<DateTime>(nullable: false),
                    Read = table.Column<bool>(nullable: false),
                    RecipientId = table.Column<long>(nullable: false),
                    AuthorId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Mails_Characters_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Characters",
                        principalColumn: "CharacterId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Mails_Characters_RecipientId",
                        column: x => x.RecipientId,
                        principalTable: "Characters",
                        principalColumn: "CharacterId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Mails_AuthorId",
                table: "Mails",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Mails_RecipientId",
                table: "Mails",
                column: "RecipientId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Mails");
        }
    }
}
