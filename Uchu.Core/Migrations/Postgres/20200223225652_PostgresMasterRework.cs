using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Uchu.Core.Migrations
{
    [SuppressMessage("ReSharper", "CA1062")]
    public partial class PostgresMasterRework : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Specifications");

            migrationBuilder.DropTable(
                name: "WorldServerRequests");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Specifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ActiveUserCount = table.Column<long>(nullable: false),
                    MaxUserCount = table.Column<long>(nullable: false),
                    Port = table.Column<int>(nullable: false),
                    ServerType = table.Column<int>(nullable: false),
                    ZoneCloneId = table.Column<long>(nullable: false),
                    ZoneId = table.Column<int>(nullable: false),
                    ZoneInstanceId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Specifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WorldServerRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    SpecificationId = table.Column<Guid>(nullable: false),
                    State = table.Column<int>(nullable: false),
                    ZoneId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorldServerRequests", x => x.Id);
                });
        }
    }
}
