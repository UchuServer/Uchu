using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Uchu.Core.Migrations
{
    public partial class MakeTasksRequired : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<List<int>>(
                name: "Tasks",
                table: "Missions",
                nullable: false,
                oldClrType: typeof(List<int>),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<List<int>>(
                name: "Tasks",
                table: "Missions",
                nullable: true,
                oldClrType: typeof(List<int>));
        }
    }
}
