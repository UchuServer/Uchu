using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Uchu.Core
{
    public class CDClient : IDisposable
    {
        public CDClient()
        {
            var file = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "CDClient.db");

            Connection = new SQLiteConnection($"Data Source={file};Version=3");

            Connection.Open();
        }

        public SQLiteConnection Connection { get; }

        public void Dispose()
        {
            Connection?.Dispose();
        }

        public async Task<ComponentsRegistryRow[]> GetComponentsAsync(int lot)
        {
            var list = new List<ComponentsRegistryRow>();

            using (var cmd = new SQLiteCommand("SELECT * FROM ComponentsRegistry WHERE id = ?", Connection))
            {
                cmd.Parameters.Add(new SQLiteParameter {Value = lot});

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                        list.Add(new ComponentsRegistryRow
                        {
                            LOT = reader.GetInt32(0),
                            ComponentType = reader.GetInt32(1),
                            ComponentId = reader.GetInt32(2)
                        });
                }
            }

            return list.ToArray();
        }

        public async Task<long> GetComponentIdAsync(int lot, int type)
        {
            using (var cmd =
                new SQLiteCommand("SELECT component_id FROM ComponentsRegistry WHERE id = ? AND component_type = ?",
                    Connection))
            {
                cmd.Parameters.AddRange(new[]
                {
                    new SQLiteParameter {Value = lot},
                    new SQLiteParameter {Value = type}
                });

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (!await reader.ReadAsync())
                        return -1;

                    return reader.GetInt64(0);
                }
            }
        }

        public async Task<MissionNPCComponentRow[]> GetNPCMissionsAsync(int lot)
        {
            var list = new List<MissionNPCComponentRow>();

            var str = new StringBuilder();

            str.Append("SELECT C.* FROM MissionNPCComponent AS C ");
            str.Append("JOIN Missions AS M ON C.missionID = M.id ");
            str.Append("WHERE C.id = ? ORDER BY UISortOrder");

            using (var cmd = new SQLiteCommand(str.ToString(), Connection))
            {
                cmd.Parameters.Add(new SQLiteParameter {Value = lot});

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                        list.Add(new MissionNPCComponentRow
                        {
                            LOT = reader.GetInt32(0),
                            MissionId = reader.GetInt32(1),
                            OffersMission = reader.GetBoolean(2),
                            AcceptsMission = reader.GetBoolean(3),
                            GateVersion = reader.IsDBNull(4) ? "" : reader.GetString(4)
                        });
                }
            }

            return list.ToArray();
        }

        public async Task<MissionsRow> GetMissionAsync(int id)
        {
            using (var cmd = new SQLiteCommand("SELECT * FROM Missions WHERE id = ?", Connection))
            {
                cmd.Parameters.Add(new SQLiteParameter {Value = id});

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (!await reader.ReadAsync())
                        return null;

                    var row = new MissionsRow
                    {
                        MissionId = reader.GetInt32(0),
                        Type = reader.IsDBNull(1) ? "" : reader.GetString(1),
                        Subtype = reader.IsDBNull(2) ? "" : reader.GetString(2),
                        UserInterfaceSortOrder = reader.IsDBNull(3) ? -1 : reader.GetInt32(3),
                        OffererObjectId = reader.GetInt32(4),
                        TargetObjectId = reader.GetInt32(5),
                        CurrencyReward = reader.GetInt64(6),
                        LegoScoreReward = reader.GetInt32(7),
                        ReputationReward = reader.IsDBNull(8) ? 0 : reader.GetInt64(8),
                        IsChoiceReward = reader.GetBoolean(9),
                        FirstItemReward = reader.GetInt32(10),
                        FirstItemRewardCount = reader.GetInt32(11),
                        SecondItemReward = reader.GetInt32(12),
                        SecondItemRewardCount = reader.GetInt32(13),
                        ThirdItemReward = reader.GetInt32(14),
                        ThirdItemRewardCount = reader.GetInt32(15),
                        FourthItemReward = reader.GetInt32(16),
                        FourthItemRewardCount = reader.GetInt32(17),
                        FirstEmoteReward = reader.GetInt32(18),
                        SecondEmoteReward = reader.GetInt32(19),
                        ThirdEmoteReward = reader.IsDBNull(20) ? -1 : reader.GetInt32(20),
                        FourthEmoteReward = reader.IsDBNull(21) ? -1 : reader.GetInt32(21),
                        MaximumImaginationReward = reader.IsDBNull(22) ? 0 : reader.GetInt32(22),
                        MaximumHealthReward = reader.IsDBNull(23) ? 0 : reader.GetInt32(23),
                        MaximumInventoryReward = reader.IsDBNull(24) ? 0 : reader.GetInt32(24),
                        MaximumModelsReward = reader.IsDBNull(25) ? 0 : reader.GetInt32(25),
                        MaximumWidgetsReward = reader.IsDBNull(26) ? 0 : reader.GetInt32(26),
                        MaximumWalletReward = reader.IsDBNull(27) ? 0 : reader.GetInt64(27),
                        IsRepeatable = reader.GetBoolean(28),
                        RepeatableCurrencyReward = reader.IsDBNull(29) ? 0 : reader.GetInt64(29),
                        FirstRepeatableItemReward = reader.IsDBNull(30) ? -1 : reader.GetInt32(30),
                        FirstRepeatableItemRewardCount = reader.IsDBNull(32) ? 0 : reader.GetInt32(31),
                        SecondRepeatableItemReward = reader.IsDBNull(32) ? -1 : reader.GetInt32(32),
                        SecondRepeatableItemRewardCount = reader.IsDBNull(33) ? 0 : reader.GetInt32(33),
                        ThirdRepeatableItemReward = reader.IsDBNull(34) ? -1 : reader.GetInt32(34),
                        ThirdRepeatableItemRewardCount = reader.IsDBNull(35) ? 0 : reader.GetInt32(35),
                        FourthRepeatableItemReward = reader.IsDBNull(36) ? -1 : reader.GetInt32(36),
                        FourthRepeatableItemRewardCount = reader.IsDBNull(37) ? 0 : reader.GetInt32(37),
                        TimeLimit = reader.IsDBNull(38) ? -1 : reader.GetInt32(38),
                        IsMission = reader.GetBoolean(39),
                        MissionIconId = reader.IsDBNull(40) ? -1 : reader.GetInt32(40),
                        // TODO: Look more into this
                        PrerequiredMissions = reader.IsDBNull(41)
                            ? new int[0]
                            : reader.GetValue(41).ToString().Trim().Split(',', '|')
                                .Select(s => s.Trim().Split(':')[0].Trim()).Where(s => !string.IsNullOrEmpty(s))
                                .Select(int.Parse).ToArray(),
                        Localize = reader.GetBoolean(42),
                        IsInMotd = reader.GetBoolean(43),
                        CooldownTime = reader.IsDBNull(44) ? 0 : reader.GetInt64(44),
                        IsRandom = reader.GetBoolean(45),
                        RandomPool = reader.IsDBNull(46) ? "" : reader.GetString(46),
                        PrerequiredUserInterfaceId = reader.IsDBNull(47) ? -1 : reader.GetInt32(47),
                        GateVersion = reader.IsDBNull(48) ? "" : reader.GetString(48),
                        HUDStates = reader.IsDBNull(49) ? "" : reader.GetString(49),
                        LocStatus = reader.IsDBNull(50) ? -1 : reader.GetInt32(50),
                        BankInventoryReward = reader.IsDBNull(51) ? -1 : reader.GetInt32(51)
                    };
                    return row;
                }
            }
        }

        public async Task<List<MissionTasksRow>> GetMissionTasksAsync(int missionId)
        {
            var list = new List<MissionTasksRow>();

            using (var cmd = new SQLiteCommand("SELECT * FROM MissionTasks WHERE id = ?", Connection))
            {
                cmd.Parameters.Add(new SQLiteParameter {Value = missionId});

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var targets = new List<object>();

                        if (!reader.IsDBNull(3))
                            targets.Add(reader.GetInt32(3));

                        if (!reader.IsDBNull(4))
                            targets.AddRange(reader.GetString(4).Trim().Split(',').Where(c => !string.IsNullOrEmpty(c))
                                .Select(c => int.TryParse(c.Trim(), out var num) ? (object) num : c.Trim()));

                        list.Add(new MissionTasksRow
                        {
                            MissionId = reader.GetInt32(0),
                            LocStatus = reader.GetInt32(1),
                            TaskType = reader.GetInt32(2),
                            Targets = targets.ToArray(),
                            TargetValue = reader.GetInt32(5),
                            TaskParameter = reader.IsDBNull(6) ? "" : reader.GetString(6),
                            LargeTaskIcon = reader.IsDBNull(7) ? "" : reader.GetString(7),
                            IconId = reader.IsDBNull(8) ? -1 : reader.GetInt32(8),
                            UId = reader.GetInt32(9),
                            LargeTaskIconId = reader.IsDBNull(10) ? -1 : reader.GetInt32(10),
                            Localize = reader.GetBoolean(11),
                            GateVersion = reader.IsDBNull(12) ? "" : reader.GetString(12)
                        });
                    }
                }
            }

            return list;
        }

        public async Task<List<MissionTasksRow>> GetMissionTasksWithTargetAsync(int lot)
        {
            var list = new List<MissionTasksRow>();

            using (var cmd = new SQLiteCommand("SELECT * FROM MissionTasks WHERE targetGroup LIKE ? OR target = ?",
                Connection))
            {
                cmd.Parameters.AddRange(new[]
                {
                    new SQLiteParameter {Value = $"%{lot}%"},
                    new SQLiteParameter {Value = lot}
                });

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var targets = new List<object>();

                        if (!reader.IsDBNull(3))
                            targets.Add(reader.GetInt32(3));

                        if (!reader.IsDBNull(4))
                            targets.AddRange(reader.GetString(4).Trim().Split(',').Where(c => !string.IsNullOrEmpty(c))
                                .Select(c => int.TryParse(c.Trim(), out var num) ? (object) num : c.Trim()));

                        if (!targets.Contains(lot))
                            continue;

                        list.Add(new MissionTasksRow
                        {
                            MissionId = reader.GetInt32(0),
                            LocStatus = reader.GetInt32(1),
                            TaskType = reader.GetInt32(2),
                            Targets = targets.ToArray(),
                            TargetValue = reader.GetInt32(5),
                            TaskParameter = reader.IsDBNull(6) ? "" : reader.GetString(6),
                            LargeTaskIcon = reader.IsDBNull(7) ? "" : reader.GetString(7),
                            IconId = reader.IsDBNull(8) ? -1 : reader.GetInt32(8),
                            UId = reader.GetInt32(9),
                            LargeTaskIconId = reader.IsDBNull(10) ? -1 : reader.GetInt32(10),
                            Localize = reader.GetBoolean(11),
                            GateVersion = reader.IsDBNull(12) ? "" : reader.GetString(12)
                        });
                    }
                }
            }

            return list;
        }

        public async Task<InventoryComponentRow[]> GetInventoryItemsAsync(int id)
        {
            var list = new List<InventoryComponentRow>();

            using (var cmd = new SQLiteCommand("SELECT * FROM InventoryComponent WHERE id = ?", Connection))
            {
                cmd.Parameters.Add(new SQLiteParameter {Value = id});

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                        list.Add(new InventoryComponentRow
                        {
                            InventoryId = reader.GetInt32(0),
                            ItemId = reader.GetInt32(1),
                            ItemCount = reader.GetInt32(2),
                            Equipped = reader.GetBoolean(3)
                        });
                }
            }

            return list.ToArray();
        }

        public async Task<SkillBehaviorRow> GetSkillBehaviorAsync(int skillId)
        {
            using (var cmd = new SQLiteCommand("SELECT * FROM SkillBehavior WHERE skillID = ?", Connection))
            {
                cmd.Parameters.Add(new SQLiteParameter {Value = skillId});

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (!await reader.ReadAsync())
                        return null;

                    return new SkillBehaviorRow
                    {
                        SkillId = reader.GetInt32(0),
                        LocStatus = reader.GetInt32(1),
                        BehaviorId = reader.GetInt32(2),
                        ImaginationCost = reader.GetInt32(3),
                        CooldownGroup = reader.GetInt32(4),
                        Cooldown = reader.GetFloat(5),
                        InNPCEditor = reader.GetBoolean(6),
                        SkillIcon = reader.GetInt32(7),
                        OOMSkillId = reader.IsDBNull(8) ? "" : reader.GetString(8),
                        OOMBehaviorEffectId = reader.IsDBNull(9) ? -1 : reader.GetInt32(9),
                        CastTypeDesc = reader.IsDBNull(10) ? -1 : reader.GetInt32(10),
                        ImaginationBonusUserInterface = reader.IsDBNull(11) ? -1 : reader.GetInt32(11),
                        LifeBonusUserInterface = reader.IsDBNull(12) ? -1 : reader.GetInt32(12),
                        ArmorBonusUserInterface = reader.IsDBNull(13) ? -1 : reader.GetInt32(13),
                        DamageUserInterface = reader.IsDBNull(14) ? -1 : reader.GetInt32(14),
                        HideIcon = reader.GetBoolean(15),
                        Localize = reader.GetBoolean(16),
                        GateVersion = reader.IsDBNull(17) ? "" : reader.GetString(17),
                        CancelType = reader.IsDBNull(18) ? -1 : reader.GetInt32(18)
                    };
                }
            }
        }

        public async Task<BehaviorTemplateRow> GetBehaviorTemplateAsync(int behaviorId)
        {
            using (var cmd = new SQLiteCommand("SELECT * FROM BehaviorTemplate WHERE behaviorID = ?", Connection))
            {
                cmd.Parameters.Add(new SQLiteParameter {Value = behaviorId});

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (!await reader.ReadAsync())
                        return null;

                    return new BehaviorTemplateRow
                    {
                        BehaviorId = reader.GetInt32(0),
                        TemplateId = reader.GetInt32(1),
                        EffectId = reader.IsDBNull(2) ? -1 : reader.GetInt32(2),
                        EffectHandle = reader.IsDBNull(3) ? "" : reader.GetString(3)
                    };
                }
            }
        }

        public async Task<BehaviorParameterRow[]> GetBehaviorParametersAsync(int behaviorId)
        {
            var list = new List<BehaviorParameterRow>();

            using (var cmd = new SQLiteCommand("SELECT * FROM BehaviorParameter WHERE behaviorID = ?", Connection))
            {
                cmd.Parameters.Add(new SQLiteParameter {Value = behaviorId});

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                        list.Add(new BehaviorParameterRow
                        {
                            BehaviorId = reader.GetInt32(0),
                            ParameterId = reader.GetString(1),
                            Value = reader.GetFloat(2)
                        });
                }
            }

            return list.ToArray();
        }

        public async Task<BehaviorParameterRow> GetBehaviorParameterAsync(int behaviorId, string name)
        {
            using (var cmd =
                new SQLiteCommand("SELECT * FROM BehaviorParameter WHERE behaviorID = ? AND parameterID = ?",
                    Connection))
            {
                cmd.Parameters.AddRange(new[]
                {
                    new SQLiteParameter {Value = behaviorId},
                    new SQLiteParameter {Value = name}
                });

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (!await reader.ReadAsync())
                        return null;

                    return new BehaviorParameterRow
                    {
                        BehaviorId = reader.GetInt32(0),
                        ParameterId = reader.GetString(1),
                        Value = reader.GetFloat(2)
                    };
                }
            }
        }

        public async Task<LootMatrixRow[]> GetDropsForObjectAsync(int lot)
        {
            var list = new List<LootMatrixRow>();

            var str = new StringBuilder();

            str.Append("SELECT * FROM LootMatrix WHERE LootMatrixIndex = (");
            str.Append("SELECT LootMatrixIndex FROM DestructibleComponent WHERE id = (");
            str.Append("SELECT component_id FROM ComponentsRegistry WHERE id = ? AND component_type = 7))");

            using (var cmd = new SQLiteCommand(str.ToString(), Connection))
            {
                cmd.Parameters.Add(new SQLiteParameter {Value = lot});

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                        list.Add(new LootMatrixRow
                        {
                            LootMatrixIndex = reader.GetInt32(0),
                            LootTableIndex = reader.GetInt32(1),
                            RarityTableIndex = reader.GetInt32(2),
                            Percent = reader.GetFloat(3),
                            MinDrops = reader.GetInt32(4),
                            MaxDrops = reader.GetInt32(5),
                            Id = reader.GetInt32(6),
                            FlagId = reader.IsDBNull(7) ? -1 : reader.GetInt32(7),
                            GateVersion = reader.IsDBNull(8) ? "" : reader.GetString(8)
                        });
                }
            }

            return list.ToArray();
        }

        public async Task<LootTableRow[]> GetItemDropsAsync(int index)
        {
            var list = new List<LootTableRow>();

            using (var cmd = new SQLiteCommand("SELECT * FROM LootTable WHERE LootTableIndex = ?", Connection))
            {
                cmd.Parameters.Add(new SQLiteParameter {Value = index});

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                        list.Add(new LootTableRow
                        {
                            ItemId = reader.GetInt32(0),
                            LootTableIndex = reader.GetInt32(1),
                            Id = reader.GetInt32(2),
                            IsMissionDrop = reader.GetBoolean(3),
                            SortPriority = reader.GetInt32(4)
                        });
                }
            }

            return list.ToArray();
        }

        public async Task<ItemComponentRow> GetItemComponentAsync(int id)
        {
            using (var cmd = new SQLiteCommand("SELECT * FROM ItemComponent WHERE id = ?", Connection))
            {
                cmd.Parameters.Add(new SQLiteParameter {Value = id});

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (!await reader.ReadAsync())
                        return null;

                    return new ItemComponentRow
                    {
                        ItemId = reader.GetInt32(0),
                        EquipLocation = reader.IsDBNull(1) ? "" : reader.GetString(1),
                        BaseValue = reader.IsDBNull(2) ? -1 : reader.GetInt32(2),
                        IsKitPiece = reader.GetBoolean(3),
                        Rarity = reader.IsDBNull(4) ? -1 : reader.GetInt32(4),
                        ItemType = reader.IsDBNull(5) ? 0 : reader.GetInt32(5),
                        ItemInfo = reader.IsDBNull(6) ? -1 : reader.GetInt64(6),
                        IsInLootTable = reader.GetBoolean(7),
                        IsInVendor = reader.GetBoolean(8),
                        IsUnique = reader.GetBoolean(9),
                        IsBoundOnPickup = reader.GetBoolean(10),
                        IsBoundOnEquip = reader.GetBoolean(11),
                        RequiredFlagId = reader.IsDBNull(12) ? -1 : reader.GetInt32(12),
                        RequiredSpecialtyId = reader.IsDBNull(13) ? -1 : reader.GetInt32(13),
                        RequiredSpecialtyRank = reader.IsDBNull(14) ? -1 : reader.GetInt32(14),
                        RequiredAchievementId = reader.IsDBNull(15) ? -1 : reader.GetInt32(15),
                        StackSize = reader.IsDBNull(16) ? -1 : reader.GetInt32(16),
                        Color = reader.IsDBNull(17) ? -1 : reader.GetInt32(17),
                        Decal = reader.IsDBNull(18) ? -1 : reader.GetInt32(18),
                        OffsetGroupId = reader.IsDBNull(19) ? -1 : reader.GetInt32(19),
                        BuildTypes = reader.IsDBNull(20) ? -1 : reader.GetInt32(20),
                        RequiredPrecondition = reader.IsDBNull(21) ? "" : reader.GetString(21),
                        AnimationFlag = reader.IsDBNull(22) ? -1 : reader.GetInt32(22),
                        EquipEffects = reader.IsDBNull(23) ? -1 : reader.GetInt32(23),
                        ReadyForQA = reader.GetBoolean(24),
                        ItemRating = reader.IsDBNull(25) ? -1 : reader.GetInt32(25),
                        IsTwoHanded = reader.GetBoolean(26),
                        MinimumNumberRequired = reader.IsDBNull(27) ? -1 : reader.GetInt32(27),
                        DelResIndex = reader.IsDBNull(28) ? -1 : reader.GetInt32(28),
                        AmmunitionLOT = reader.IsDBNull(29) ? -1 : reader.GetInt32(29),
                        AltAmmunitionCost = reader.IsDBNull(30) ? -1 : reader.GetInt32(30),
                        SubItems = reader.IsDBNull(31)
                            ? new int[0]
                            : reader.GetString(31).Trim().Split(',').Where(s => !string.IsNullOrEmpty(s.Trim()))
                                .Select(s => int.Parse(s.Trim())).ToArray(),
                        AudioEventUse = reader.IsDBNull(32) ? "" : reader.GetString(32),
                        NoEquipAnimation = reader.GetBoolean(33),
                        CommendationLOT = reader.IsDBNull(34) ? -1 : reader.GetInt32(34),
                        CommendationCost = reader.IsDBNull(35) ? -1 : reader.GetInt32(35),
                        AudioEquipMetaEventSet = reader.IsDBNull(36) ? "" : reader.GetString(36),
                        CurrencyCosts = reader.IsDBNull(37) ? "" : reader.GetString(37), // TODO: parse
                        IngredientInfo = reader.IsDBNull(38) ? "" : reader.GetString(38),
                        LocStatus = reader.IsDBNull(39) ? -1 : reader.GetInt32(39),
                        ForgeType = reader.IsDBNull(40) ? -1 : reader.GetInt32(40),
                        SellMultiplier = reader.IsDBNull(41) ? -1 : reader.GetFloat(41)
                    };
                }
            }
        }

        public async Task<RocketLaunchpadControlComponentRow> GetLaunchpadComponent(int id)
        {
            using (var cmd =
                new SQLiteCommand("SELECT * FROM RocketLaunchpadControlComponent WHERE id = ?", Connection))
            {
                cmd.Parameters.Add(new SQLiteParameter {Value = id});

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (!await reader.ReadAsync())
                        return null;

                    return new RocketLaunchpadControlComponentRow
                    {
                        LaunchpadId = reader.GetInt32(0),
                        TargetZone = reader.GetInt32(1),
                        DefaultZoneId = reader.GetInt32(2),
                        TargetScene = reader.IsDBNull(3) ? "" : reader.GetString(3),
                        GmLevel = reader.GetInt32(4),
                        PlayerAnimation = reader.IsDBNull(5) ? "" : reader.GetString(5),
                        RocketAnimation = reader.IsDBNull(6) ? "" : reader.GetString(6),
                        LaunchMusic = reader.IsDBNull(7) ? "" : reader.GetString(7),
                        UseLaunchPrecondition = reader.GetBoolean(8),
                        UseLandingPrecondition = reader.GetBoolean(9),
                        LaunchPrecondition = reader.IsDBNull(10) ? "" : reader.GetString(10),
                        LandingPrecondition = reader.IsDBNull(11) ? "" : reader.GetString(11),
                        LandingSpawnpointName = reader.IsDBNull(12) ? "" : reader.GetString(12)
                    };
                }
            }
        }

        public async Task<ScriptComponentRow> GetScriptComponent(int id)
        {
            using (var cmd = new SQLiteCommand("SELECT * FROM ScriptComponent WHERE id = ?", Connection))
            {
                cmd.Parameters.Add(new SQLiteParameter {Value = id});

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (!await reader.ReadAsync())
                        return null;
                    
                    return new ScriptComponentRow
                    {
                        ScriptId = reader.GetInt32(0),
                        ServerScriptName = reader.IsDBNull(1) ? null : reader.GetString(1),
                        ClientScriptName = reader.IsDBNull(2) ? null : reader.GetString(2)
                    };
                }
            }
        }

        public async Task<int> GetTemplateFromName(string name)
        {
            using (var cmd = new SQLiteCommand("SELECT id FROM Objects WHERE name = '" + name + "'", Connection))
            {
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (!await reader.ReadAsync())
                        return 2620;

                    return reader.GetInt32(0);
                }
            }
        }

        public async Task<string> GetBrickColorName(int brickColor)
        {
            using (var cmd = new SQLiteCommand("SELECT description FROM BrickColors WHERE id = ?", Connection))
            {
                cmd.Parameters.Add(new SQLiteParameter {Value = brickColor});

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (!await reader.ReadAsync())
                        return "";

                    return reader.GetString(0);
                }
            }
        }
    }
}