using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Uchu.Core
{
    public class CDClient : IDisposable
    {
        public SQLiteConnection Connection { get; }

        public CDClient()
        {
            var file = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "CDClient.db");

            Connection = new SQLiteConnection($"Data Source={file};Version=3");

            Connection.Open();
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
                    {
                        list.Add(new ComponentsRegistryRow
                        {
                            LOT = reader.GetInt32(0),
                            ComponentType = reader.GetInt32(1),
                            ComponentId = reader.GetInt32(2)
                        });
                    }
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

        public async Task<MissionNPCComponentRow[]> GetNPCMissions(int lot)
        {
            var list = new List<MissionNPCComponentRow>();

            using (var cmd = new SQLiteCommand("SELECT * FROM MissionNPCComponent WHERE id = ?", Connection))
            {
                cmd.Parameters.Add(new SQLiteParameter {Value = lot});

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
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

                    return new MissionsRow
                    {
                        MissionId = reader.GetInt32(0),
                        Type = reader.IsDBNull(1) ? "" : reader.GetString(1),
                        Subtype = reader.IsDBNull(2) ? "" : reader.GetString(2),
                        UserInterfaceSortOrder = reader.GetInt32(3),
                        OffererObjectId = reader.GetInt32(4),
                        TargetObjectId = reader.GetInt32(5),
                        CurrencyReward = reader.GetInt64(6),
                        LegoScoreReward = reader.GetInt32(7),
                        ReputationReward = reader.GetInt64(8),
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
                        ThirdEmoteReward = reader.GetInt32(20),
                        FourthEmoteReward = reader.GetInt32(21),
                        MaximumImaginationReward = reader.GetInt32(22),
                        MaximumHealthReward = reader.GetInt32(23),
                        MaximumInventoryReward = reader.GetInt32(24),
                        MaximumModelsReward = reader.GetInt32(25),
                        MaximumWidgetsReward = reader.GetInt32(26),
                        MaximumWalletReward = reader.GetInt64(27),
                        IsRepeatable = reader.GetBoolean(28),
                        RepeatableCurrencyReward = reader.GetInt64(29),
                        FirstRepeatableItemReward = reader.GetInt32(30),
                        FirstRepeatableItemRewardCount = reader.GetInt32(31),
                        SecondRepeatableItemReward = reader.GetInt32(32),
                        SecondRepeatableItemRewardCount = reader.GetInt32(33),
                        ThirdRepeatableItemReward = reader.GetInt32(34),
                        ThirdRepeatableItemRewardCount = reader.GetInt32(35),
                        FourthRepeatableItemReward = reader.GetInt32(36),
                        FourthRepeatableItemRewardCount = reader.GetInt32(37),
                        TimeLimit = reader.IsDBNull(38) ? -1 : reader.GetInt32(38),
                        IsMission = reader.GetBoolean(39),
                        MissionIconId = reader.IsDBNull(40) ? -1 : reader.GetInt32(40),
                        PrerequiredMissions = reader.IsDBNull(41) ? new int[0] :
                            reader.GetString(41).Trim().Split(',').Where(c => !string.IsNullOrEmpty(c))
                                .Select(c => int.Parse(c.Trim())).ToArray(),
                        Localize = reader.GetBoolean(42),
                        IsInMotd = reader.GetBoolean(43),
                        CooldownTime = reader.IsDBNull(44) ? -1 : reader.GetInt64(44),
                        IsRandom = reader.GetBoolean(45),
                        RandomPool = reader.IsDBNull(46) ? "" : reader.GetString(46),
                        PrerequiredUserInterfaceId = reader.IsDBNull(47) ? -1 : reader.GetInt32(47),
                        GateVersion = reader.IsDBNull(48) ? "" : reader.GetString(48),
                        HUDStates = reader.IsDBNull(49) ? "" : reader.GetString(49),
                        LocStatus = reader.GetInt32(50),
                        BankInventoryReward = reader.IsDBNull(51) ? -1 : reader.GetInt32(51)
                    };
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
                        var targets = new List<int> {reader.GetInt32(3)};

                        if (!reader.IsDBNull(4))
                            targets.AddRange(reader.GetString(4).Trim().Split(',').Where(c => !string.IsNullOrEmpty(c))
                                .Select(c => int.Parse(c.Trim())));

                        list.Add(new MissionTasksRow
                        {
                            MissionId = reader.GetInt32(0),
                            LocStatus = reader.GetInt32(1),
                            TaskType = reader.GetInt32(2),
                            TargetLOTs = targets.ToArray(),
                            TargetValue = reader.GetInt32(5),
                            TaskParameter = reader.IsDBNull(6) ? "" : reader.GetString(6),
                            LargeTaskIcon = reader.IsDBNull(7) ? "" : reader.GetString(7),
                            IconId = reader.GetInt32(8),
                            UId = reader.GetInt32(9),
                            LargeTaskIconId = reader.GetInt32(10),
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
                    {
                        list.Add(new InventoryComponentRow
                        {
                            InventoryId = reader.GetInt32(0),
                            ItemId = reader.GetInt32(1),
                            ItemCount = reader.GetInt32(2),
                            Equipped = reader.GetBoolean(3)
                        });
                    }
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
                    {
                        list.Add(new BehaviorParameterRow
                        {
                            BehaviorId = reader.GetInt32(0),
                            ParameterId = reader.GetString(1),
                            Value = reader.GetFloat(2)
                        });
                    }
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

        public void Dispose()
        {
            Connection?.Dispose();
        }
    }
}