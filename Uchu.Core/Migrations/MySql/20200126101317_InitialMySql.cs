using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Uchu.Core.Migrations.MySql
{
    public partial class InitialMySql : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SessionCaches",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Key = table.Column<string>(nullable: true),
                    CharacterId = table.Column<long>(nullable: false),
                    UserId = table.Column<long>(nullable: false),
                    ZoneId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SessionCaches", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Specifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ServerType = table.Column<int>(nullable: false),
                    Port = table.Column<int>(nullable: false),
                    MaxUserCount = table.Column<uint>(nullable: false),
                    ActiveUserCount = table.Column<uint>(nullable: false),
                    ZoneId = table.Column<ushort>(nullable: false),
                    ZoneCloneId = table.Column<uint>(nullable: false),
                    ZoneInstanceId = table.Column<ushort>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Specifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Username = table.Column<string>(maxLength: 33, nullable: false),
                    Password = table.Column<string>(maxLength: 60, nullable: false),
                    Banned = table.Column<bool>(nullable: false),
                    BannedReason = table.Column<string>(nullable: true),
                    CustomLockout = table.Column<string>(nullable: true),
                    GameMasterLevel = table.Column<int>(nullable: false),
                    FreeToPlay = table.Column<bool>(nullable: false),
                    FirstTimeOnSubscription = table.Column<bool>(nullable: false),
                    CharacterIndex = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "WorldServerRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ZoneId = table.Column<ushort>(nullable: false),
                    State = table.Column<int>(nullable: false),
                    SpecificationId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorldServerRequests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Characters",
                columns: table => new
                {
                    CharacterId = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 33, nullable: false),
                    CustomName = table.Column<string>(maxLength: 33, nullable: false),
                    NameRejected = table.Column<bool>(nullable: false),
                    FreeToPlay = table.Column<bool>(nullable: false),
                    ShirtColor = table.Column<long>(nullable: false),
                    ShirtStyle = table.Column<long>(nullable: false),
                    PantsColor = table.Column<long>(nullable: false),
                    HairStyle = table.Column<long>(nullable: false),
                    HairColor = table.Column<long>(nullable: false),
                    Lh = table.Column<long>(nullable: false),
                    Rh = table.Column<long>(nullable: false),
                    EyebrowStyle = table.Column<long>(nullable: false),
                    EyeStyle = table.Column<long>(nullable: false),
                    MouthStyle = table.Column<long>(nullable: false),
                    LastZone = table.Column<int>(nullable: false),
                    LastInstance = table.Column<int>(nullable: false),
                    LastClone = table.Column<long>(nullable: false),
                    LastActivity = table.Column<long>(nullable: false),
                    Level = table.Column<long>(nullable: false),
                    UniverseScore = table.Column<long>(nullable: false),
                    Currency = table.Column<long>(nullable: false),
                    MaximumHealth = table.Column<int>(nullable: false),
                    CurrentHealth = table.Column<int>(nullable: false),
                    BaseHealth = table.Column<int>(nullable: false),
                    MaximumArmor = table.Column<int>(nullable: false),
                    CurrentArmor = table.Column<int>(nullable: false),
                    MaximumImagination = table.Column<int>(nullable: false),
                    CurrentImagination = table.Column<int>(nullable: false),
                    BaseImagination = table.Column<int>(nullable: false),
                    TotalCurrencyCollected = table.Column<long>(nullable: false),
                    TotalBricksCollected = table.Column<long>(nullable: false),
                    TotalSmashablesSmashed = table.Column<long>(nullable: false),
                    TotalQuickBuildsCompleted = table.Column<long>(nullable: false),
                    TotalEnemiesSmashed = table.Column<long>(nullable: false),
                    TotalRocketsUsed = table.Column<long>(nullable: false),
                    TotalMissionsCompleted = table.Column<long>(nullable: false),
                    TotalPetsTamed = table.Column<long>(nullable: false),
                    TotalImaginationPowerUpsCollected = table.Column<long>(nullable: false),
                    TotalLifePowerUpsCollected = table.Column<long>(nullable: false),
                    TotalArmorPowerUpsCollected = table.Column<long>(nullable: false),
                    TotalDistanceTraveled = table.Column<long>(nullable: false),
                    TotalSuicides = table.Column<long>(nullable: false),
                    TotalDamageTaken = table.Column<long>(nullable: false),
                    TotalDamageHealed = table.Column<long>(nullable: false),
                    TotalArmorRepaired = table.Column<long>(nullable: false),
                    TotalImaginationRestored = table.Column<long>(nullable: false),
                    TotalImaginationUsed = table.Column<long>(nullable: false),
                    TotalDistanceDriven = table.Column<long>(nullable: false),
                    TotalTimeAirborne = table.Column<long>(nullable: false),
                    TotalRacingImaginationPowerUpsCollected = table.Column<long>(nullable: false),
                    TotalRacingImaginationCratesSmashed = table.Column<long>(nullable: false),
                    TotalRacecarBoostsActivated = table.Column<long>(nullable: false),
                    TotalRacecarWrecks = table.Column<long>(nullable: false),
                    TotalRacingSmashablesSmashed = table.Column<long>(nullable: false),
                    TotalRacesFinished = table.Column<long>(nullable: false),
                    TotalFirstPlaceFinishes = table.Column<long>(nullable: false),
                    LandingByRocket = table.Column<bool>(nullable: false),
                    LaunchedRocketFrom = table.Column<ushort>(nullable: false),
                    Rocket = table.Column<string>(maxLength: 30, nullable: true),
                    UserId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Characters", x => x.CharacterId);
                    table.ForeignKey(
                        name: "FK_Characters_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Friends",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IsAccepted = table.Column<bool>(nullable: false),
                    IsDeclined = table.Column<bool>(nullable: false),
                    IsBestFriend = table.Column<bool>(nullable: false),
                    RequestHasBeenSent = table.Column<bool>(nullable: false),
                    RequestingBestFriend = table.Column<bool>(nullable: false),
                    FriendId = table.Column<long>(nullable: false),
                    FriendTwoId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Friends", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Friends_Characters_FriendId",
                        column: x => x.FriendId,
                        principalTable: "Characters",
                        principalColumn: "CharacterId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Friends_Characters_FriendTwoId",
                        column: x => x.FriendTwoId,
                        principalTable: "Characters",
                        principalColumn: "CharacterId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InventoryItems",
                columns: table => new
                {
                    InventoryItemId = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    LOT = table.Column<int>(nullable: false),
                    Slot = table.Column<int>(nullable: false),
                    Count = table.Column<long>(nullable: false),
                    IsBound = table.Column<bool>(nullable: false),
                    IsEquipped = table.Column<bool>(nullable: false),
                    InventoryType = table.Column<int>(nullable: false),
                    ExtraInfo = table.Column<string>(nullable: true),
                    CharacterId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryItems", x => x.InventoryItemId);
                    table.ForeignKey(
                        name: "FK_InventoryItems_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "CharacterId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Missions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    MissionId = table.Column<int>(nullable: false),
                    State = table.Column<int>(nullable: false),
                    CompletionCount = table.Column<int>(nullable: false),
                    LastCompletion = table.Column<long>(nullable: false),
                    CharacterId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Missions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Missions_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "CharacterId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UnlockedEmote",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    EmoteId = table.Column<int>(nullable: false),
                    CharacterId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnlockedEmote", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UnlockedEmote_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "CharacterId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MissionTasks",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    TaskId = table.Column<int>(nullable: false),
                    MissionId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MissionTasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MissionTasks_Missions_MissionId",
                        column: x => x.MissionId,
                        principalTable: "Missions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MissionTaskValue",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
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
                name: "IX_Characters_UserId",
                table: "Characters",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Friends_FriendId",
                table: "Friends",
                column: "FriendId");

            migrationBuilder.CreateIndex(
                name: "IX_Friends_FriendTwoId",
                table: "Friends",
                column: "FriendTwoId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryItems_CharacterId",
                table: "InventoryItems",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_Missions_CharacterId",
                table: "Missions",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_MissionTasks_MissionId",
                table: "MissionTasks",
                column: "MissionId");

            migrationBuilder.CreateIndex(
                name: "IX_MissionTaskValue_MissionTaskId",
                table: "MissionTaskValue",
                column: "MissionTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_UnlockedEmote_CharacterId",
                table: "UnlockedEmote",
                column: "CharacterId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Friends");

            migrationBuilder.DropTable(
                name: "InventoryItems");

            migrationBuilder.DropTable(
                name: "MissionTaskValue");

            migrationBuilder.DropTable(
                name: "SessionCaches");

            migrationBuilder.DropTable(
                name: "Specifications");

            migrationBuilder.DropTable(
                name: "UnlockedEmote");

            migrationBuilder.DropTable(
                name: "WorldServerRequests");

            migrationBuilder.DropTable(
                name: "MissionTasks");

            migrationBuilder.DropTable(
                name: "Missions");

            migrationBuilder.DropTable(
                name: "Characters");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
