using Microsoft.EntityFrameworkCore.Migrations;

namespace Uchu.Core.Migrations
{
    public partial class FixMissionTaskValueTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MissionTaskValue_MissionTasks_MissionTaskId",
                table: "MissionTaskValue");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MissionTaskValue",
                table: "MissionTaskValue");

            migrationBuilder.RenameTable(
                name: "MissionTaskValue",
                newName: "MissionTaskValues");

            migrationBuilder.RenameIndex(
                name: "IX_MissionTaskValue_MissionTaskId",
                table: "MissionTaskValues",
                newName: "IX_MissionTaskValues_MissionTaskId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MissionTaskValues",
                table: "MissionTaskValues",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MissionTaskValues_MissionTasks_MissionTaskId",
                table: "MissionTaskValues",
                column: "MissionTaskId",
                principalTable: "MissionTasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MissionTaskValues_MissionTasks_MissionTaskId",
                table: "MissionTaskValues");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MissionTaskValues",
                table: "MissionTaskValues");

            migrationBuilder.RenameTable(
                name: "MissionTaskValues",
                newName: "MissionTaskValue");

            migrationBuilder.RenameIndex(
                name: "IX_MissionTaskValues_MissionTaskId",
                table: "MissionTaskValue",
                newName: "IX_MissionTaskValue_MissionTaskId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MissionTaskValue",
                table: "MissionTaskValue",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MissionTaskValue_MissionTasks_MissionTaskId",
                table: "MissionTaskValue",
                column: "MissionTaskId",
                principalTable: "MissionTasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
