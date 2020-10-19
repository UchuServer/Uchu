using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Uchu.Core.Migrations.MySql
{
    [SuppressMessage("ReSharper", "CA1062")]
    public partial class MySqlAddedMail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Mails",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Subject = table.Column<string>(nullable: true),
                    Body = table.Column<string>(nullable: true),
                    AttachmentLot = table.Column<int>(nullable: false),
                    AttachmentCount = table.Column<ushort>(nullable: false),
                    AttachmentCurrency = table.Column<ulong>(nullable: false),
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
