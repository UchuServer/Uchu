using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Uchu.Core.Migrations
{
    public partial class UpdateTaskValueType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MissionTask_Missions_MissionId",
                table: "MissionTask");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MissionTask",
                table: "MissionTask");

            migrationBuilder.DropColumn(
                name: "Value",
                table: "MissionTask");

            migrationBuilder.RenameTable(
                name: "MissionTask",
                newName: "MissionTasks");

            migrationBuilder.RenameIndex(
                name: "IX_MissionTask_MissionId",
                table: "MissionTasks",
                newName: "IX_MissionTasks_MissionId");

            migrationBuilder.AddColumn<List<float>>(
                name: "Values",
                table: "MissionTasks",
                nullable: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_MissionTasks",
                table: "MissionTasks",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MissionTasks_Missions_MissionId",
                table: "MissionTasks",
                column: "MissionId",
                principalTable: "Missions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MissionTasks_Missions_MissionId",
                table: "MissionTasks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MissionTasks",
                table: "MissionTasks");

            migrationBuilder.DropColumn(
                name: "Values",
                table: "MissionTasks");

            migrationBuilder.RenameTable(
                name: "MissionTasks",
                newName: "MissionTask");

            migrationBuilder.RenameIndex(
                name: "IX_MissionTasks_MissionId",
                table: "MissionTask",
                newName: "IX_MissionTask_MissionId");

            migrationBuilder.AddColumn<float>(
                name: "Value",
                table: "MissionTask",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddPrimaryKey(
                name: "PK_MissionTask",
                table: "MissionTask",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MissionTask_Missions_MissionId",
                table: "MissionTask",
                column: "MissionId",
                principalTable: "Missions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
