using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Uchu.Core.Migrations
{
    public partial class UpdatedTaskValues : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Values",
                table: "MissionTasks");

            migrationBuilder.CreateTable(
                name: "MissionTaskValue",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Value = table.Column<float>(nullable: false),
                    Count = table.Column<int>(nullable: false),
                    MissionTaskId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MissionTaskValue", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MissionTaskValue_MissionTasks_MissionTaskId",
                        column: x => x.MissionTaskId,
                        principalTable: "MissionTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MissionTaskValue_MissionTaskId",
                table: "MissionTaskValue",
                column: "MissionTaskId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MissionTaskValue");

            migrationBuilder.AddColumn<List<float>>(
                name: "Values",
                table: "MissionTasks",
                nullable: false);
        }
    }
}
