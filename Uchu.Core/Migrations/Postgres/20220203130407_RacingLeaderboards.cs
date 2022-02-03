using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Uchu.Core.Migrations
{
    public partial class RacingLeaderboards : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BestLapTime",
                table: "ActivityScores",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Wins",
                table: "ActivityScores",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BestLapTime",
                table: "ActivityScores");

            migrationBuilder.DropColumn(
                name: "Wins",
                table: "ActivityScores");
        }
    }
}
