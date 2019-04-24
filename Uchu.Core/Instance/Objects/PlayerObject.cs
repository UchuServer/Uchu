using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Uchu.Core.Collections;
using Uchu.Core.Scripting.Lua;

namespace Uchu.Core
{
    public class PlayerObject : ISpawnableObject<ControllablePhysicsComponent>
    {
        private readonly WorldInstance _world;
        private readonly MissionParser _missionParser;

        private ReplicaPacket _replica;
        private bool _spawned;

        public Character Character { get; }
        public IPEndPoint Endpoint { get; }

        public int LOT => 1;
        public long ObjectId => Character.CharacterId;
        public LuaGameObject LuaObject { get; }
        public ControllablePhysicsComponent Physics { get; private set; }

        public Dictionary<long, int> Loot { get; }

        public PlayerObject(WorldInstance world, Character character, IPEndPoint endpoint)
        {
            _world = world;
            _missionParser = new MissionParser(_world.Server.CDClient);

            Character = character;
            Endpoint = endpoint;
            LuaObject = new LuaGameObject(this);
            Loot = new Dictionary<long, int>();
        }

        public Task SpawnAsync()
        {
            Spawn();

            return Task.CompletedTask;
        }

        public void Spawn()
        {
            if (_spawned)
                throw new InvalidOperationException();

            _replica = new ReplicaPacket
            {
                ObjectId = ObjectId,
                LOT = LOT,
                Name = Character.Name,
                Scale = 1,
                Components = new IReplicaComponent[]
                {
                    Physics = new ControllablePhysicsComponent
                    {
                        HasPosition = true,
                        Position = _world.Zone.SpawnPosition,
                        Rotation = _world.Zone.SpawnRotation
                    },
                    new DestructibleComponent(),
                    new StatsComponent
                    {
                        HasStats = true,
                        CurrentArmor = (uint) Character.CurrentArmor,
                        MaxArmor = (uint) Character.MaximumArmor,
                        CurrentHealth = (uint) Character.CurrentHealth,
                        MaxHealth = (uint) Character.MaximumHealth,
                        CurrentImagination = (uint) Character.CurrentImagination,
                        MaxImagination = (uint) Character.MaximumImagination
                    },
                    new CharacterComponent
                    {
                        Level = (uint) Character.Level,
                        Character = Character
                    },
                    new InventoryComponent
                    {
                        Items = Character.Items.Where(i => i.IsEquipped).ToArray()
                    },
                    new SkillComponent(),
                    new RenderComponent(),
                    new Component107()
                }
            };

            _world.ReplicaManager.SendConstruction(_replica);
            _spawned = true;
        }

        public void Despawn(IPEndPoint[] endpoints = null)
        {
            if (!_spawned)
                throw new InvalidOperationException();

            _world.ReplicaManager.SendDestruction(_replica, endpoints);
        }

        public void Update(IEnumerable<IReplicaComponent> components, IPEndPoint[] endpoints = null)
        {
            if (!_spawned)
                throw new InvalidOperationException();

            var updates = new Dictionary<int, IReplicaComponent>();
            var comps = new List<IReplicaComponent>();

            foreach (var comp in components)
            {
                var type =
                    comp is ControllablePhysicsComponent ? 1 :
                    comp is StatsComponent ? 7 :
                    comp is CharacterComponent ? 4 :
                    comp is InventoryComponent ? 17 :
                    comp is SkillComponent ? 9 :
                    comp is RenderComponent ? 2 :
                    comp is Component107 ? 107 :
                    throw new NotSupportedException();

                updates[type] = comp;
            }

            var charComps = new[] {1, 7, 4, 17, 9, 2, 107};

            foreach (var comp in charComps)
            {
                if (updates.ContainsKey(comp))
                {
                    if (comp == 7)
                    {
                        comps.AddRange(new[]
                        {
                            new DestructibleComponent(),
                            updates[comp]
                        });
                    }
                    else
                    {
                        comps.Add(updates[comp]);
                    }
                }
                else
                {
                    switch (comp)
                    {
                        case 1:
                            comps.Add(new ControllablePhysicsComponent());
                            break;

                        case 7:
                            comps.AddRange(new IReplicaComponent[]
                            {
                                new DestructibleComponent(),
                                new StatsComponent()
                            });
                            break;

                        case 4:
                            comps.Add(new CharacterComponent());
                            break;

                        case 17:
                            comps.Add(new InventoryComponent());
                            break;

                        case 9:
                            comps.Add(new SkillComponent());
                            break;

                        case 2:
                            comps.Add(new RenderComponent());
                            break;

                        case 107:
                            comps.Add(new Component107());
                            break;
                    }
                }
            }

            _replica.Components = comps.ToArray();

            _world.ReplicaManager.SendSerialization(_replica, endpoints);
        }

        public void Update(IReplicaComponent component, IPEndPoint[] endpoints = null)
            => Update(new[] {component}, endpoints);

        public void UpdatePosition(Vector3 position)
        {
            Physics.Position = position;
            Physics.PlatformObjectId = -1;
            Physics.IsOnGround = true;
            Physics.NegativeAngularVelocity = false;
            Physics.Velocity = null;
            Physics.AngularVelocity = null;

            Update(Physics, _world.Players.Keys.Where(ip => !ip.Equals(Endpoint)).ToArray());
        }

        public void UpdatePosition(ControllablePhysicsComponent physics)
        {
            Physics = physics;

            Update(physics, _world.Players.Keys.Where(ip => !ip.Equals(Endpoint)).ToArray());
        }

        public void Die(PlayerObject killer, KillType type = KillType.Violent)
        {
        }

        public Task DropLootAsync(PlayerObject owner)
        {
            return Task.CompletedTask;
        }

        public async Task GiveItemAsync(int lot, int count = 1, LegoDataDictionary extraInfo = null)
        {
            var item = await _world.Server.CDClient.GetItemComponentAsync(
                (int) await _world.Server.CDClient.GetComponentIdAsync(lot, 11));

            using (var ctx = new UchuContext())
            {
                var chr = await ctx.Characters.Include(c => c.Items)
                    .SingleAsync(c => c.CharacterId == ObjectId);

                var inventoryType = Utils.GetItemInventoryType((ItemType) item.ItemType);
                var items = chr.Items.Where(itm => itm.InventoryType == inventoryType).ToArray();
                var invItem = items.FirstOrDefault(itm => itm.LOT == lot && item.StackSize > itm.Count);

                if (invItem == null)
                {
                    var slot = 0;

                    if (items.Length > 0)
                    {
                        var max = items.Max(i => i.Slot);
                        slot = max + 1;

                        for (var i = 0; i < max; i++)
                        {
                            if (items.All(itm => itm.Slot != i))
                            {
                                slot = i;
                                break;
                            }
                        }
                    }

                    invItem = new InventoryItem
                    {
                        LOT = lot,
                        InventoryItemId = Utils.GenerateObjectId(),
                        IsBound = item.IsBoundOnPickup,
                        InventoryType = inventoryType,
                        Count = count,
                        Slot = slot,
                        ExtraInfo = extraInfo?.ToString()
                    };

                    chr.Items.Add(invItem);
                }
                else
                {
                    invItem.Count += count;
                }

                await ctx.SaveChangesAsync();

                _world.Server.Send(new AddItemToInventoryMessage
                {
                    ObjectId = ObjectId,
                    IsBound = invItem.IsBound,
                    IsBoundOnEquip = item.IsBoundOnEquip,
                    IsBoundOnPickup = item.IsBoundOnPickup,
                    ExtraInfo = LegoDataDictionary.FromString(invItem.ExtraInfo),
                    ItemLOT = invItem.LOT,
                    InventoryType = (int) invItem.InventoryType,
                    ItemCount = (uint) invItem.Count,
                    ItemObjectId = invItem.InventoryItemId,
                    Slot = invItem.Slot
                }, Endpoint);

                await UpdateMissionTaskAsync(invItem.LOT, MissionTaskType.ObtainItem);
            }
        }

        public async Task MoveItemAsync(long objectId, int slot, InventoryType targetInventory = InventoryType.Invalid)
        {
            using (var ctx = new UchuContext())
            {
                var chr = await ctx.Characters.Include(c => c.Items)
                    .SingleAsync(c => c.CharacterId == ObjectId);

                var invItem = chr.Items.First(itm => itm.InventoryItemId == objectId);

                if (targetInventory == InventoryType.Invalid)
                    targetInventory = invItem.InventoryType;

                invItem.Slot = slot;
                invItem.InventoryType = targetInventory;

                await ctx.SaveChangesAsync();
            }
        }

        public async Task RemoveItemAsync(long objectId, int count = 1)
        {
            using (var ctx = new UchuContext())
            {
                var chr = await ctx.Characters.Include(c => c.Items)
                    .SingleAsync(c => c.CharacterId == ObjectId);

                var invItem = chr.Items.First(itm => itm.InventoryItemId == objectId);

                invItem.Count -= count;

                if (invItem.Count <= 0)
                    chr.Items.Remove(invItem);

                await ctx.SaveChangesAsync();
            }
        }

        public async Task EquipItemAsync(long objectId)
        {
            using (var ctx = new UchuContext())
            {
                var chr = await ctx.Characters.Include(c => c.Items)
                    .SingleAsync(c => c.CharacterId == ObjectId);

                var invItem = chr.Items.First(itm => itm.InventoryItemId == objectId);

                invItem.IsEquipped = true;

                await ctx.SaveChangesAsync();

                Update(new InventoryComponent {Items = chr.Items.Where(itm => itm.IsEquipped).ToArray()});
            }
        }

        public async Task UnequipItemAsync(long objectId, long replacementId = -1)
        {
            using (var ctx = new UchuContext())
            {
                var chr = await ctx.Characters.Include(c => c.Items)
                    .SingleAsync(c => c.CharacterId == ObjectId);

                var invItem = chr.Items.First(itm => itm.InventoryItemId == objectId);

                invItem.IsEquipped = false;

                if (replacementId != -1)
                {
                    var replacementItem = chr.Items.First(itm => itm.InventoryItemId == replacementId);

                    replacementItem.IsEquipped = true;
                }

                await ctx.SaveChangesAsync();

                Update(new InventoryComponent {Items = chr.Items.Where(itm => itm.IsEquipped).ToArray()});
            }
        }

        public async Task HandleMissionResponseAsync(int missionId, long receivedId, int rewardItem = -1)
        {
            using (var ctx = new UchuContext())
            {
                var chr = await ctx.Characters.Include(c => c.Missions).ThenInclude(m => m.Tasks)
                    .SingleAsync(c => c.CharacterId == ObjectId);

                if (!chr.Missions.Exists(m => m.MissionId == missionId))
                {
                    await GiveMissionAsync(missionId);

                    return;
                }

                var chrMission = chr.Missions.Find(m => m.MissionId == missionId);

                if (await AllTasksCompletedAsync(chrMission))
                {
                    await CompleteMissionAsync(missionId);
                    await OfferMissionAsync(receivedId);
                }
            }
        }

        public async Task OfferMissionAsync(long offererId)
        {
            var obj = _world.GetObject(offererId);
            var missions = await _world.Server.CDClient.GetNPCMissionsAsync(obj.LOT);

            using (var ctx = new UchuContext())
            {
                var chr = await ctx.Characters.Include(c => c.Missions).ThenInclude(m => m.Tasks)
                    .SingleAsync(c => c.CharacterId == ObjectId);

                foreach (var npcMission in missions)
                {
                    var mission = await _world.Server.CDClient.GetMissionAsync(npcMission.MissionId);

                    if (!mission.IsMission)
                        continue;

                    var chrMission = chr.Missions.Find(m => m.MissionId == mission.MissionId);

                    if (npcMission.AcceptsMission)
                    {
                        if (chrMission != null)
                        {
                            if (chrMission.State != MissionState.Completed &&
                                await AllTasksCompletedAsync(chrMission))
                            {
                                _world.Server.Send(new OfferMissionMessage
                                {
                                    ObjectId = ObjectId,
                                    MissionId = mission.MissionId,
                                    OffererObjectId = offererId
                                }, Endpoint);

                                break;
                            }
                        }
                    }

                    if (npcMission.OffersMission)
                    {
                        var completed = chr.Missions.Where(m => m.State == MissionState.Completed).ToArray();

                        if (chrMission == null)
                        {
                            if (!await _missionParser.CheckPrerequiredMissionsAsync(mission.PrerequiredMissions,
                                completed))
                                continue;

                            _world.Server.Send(new OfferMissionMessage
                            {
                                ObjectId = ObjectId,
                                MissionId = mission.MissionId,
                                OffererObjectId = offererId
                            }, Endpoint);

                            break;
                        }
                    }
                }
            }
        }

        public async Task<bool> AllTasksCompletedAsync(Mission mission)
        {
            var tasks = await _world.Server.CDClient.GetMissionTasksAsync(mission.MissionId);

            return tasks.All(t => mission.Tasks.Find(t2 => t2.TaskId == t.UId).Values.Count >= t.TargetValue);
        }

        public async Task GiveMissionAsync(int missionId)
        {
            using (var ctx = new UchuContext())
            {
                var chr = await ctx.Characters.Include(c => c.Missions).ThenInclude(c => c.Tasks).Include(c => c.Items)
                    .SingleAsync(c => c.CharacterId == ObjectId);

                var mission = await _world.Server.CDClient.GetMissionAsync(missionId);
                var missionTasks = await _world.Server.CDClient.GetMissionTasksAsync(missionId);
                var chrMission = chr.Missions.Find(m => m.MissionId == missionId);

                if (chrMission != null)
                {
                    if (!mission.IsRepeatable)
                        return;

                    chrMission.State = MissionState.CompletedActive;

                    foreach (var task in missionTasks)
                    {
                        var chrTask = chrMission.Tasks.Find(t => t.TaskId == task.UId);
                        var values = new List<MissionTaskValue>();

                        if (task.TaskType == (int) MissionTaskType.ObtainItem)
                            values = task.Targets
                                .Where(tgt => tgt is int && chr.Items.Exists(itm => itm.LOT == (int) tgt))
                                .Select(tgt => new MissionTaskValue {Value = (float) (int) tgt}).ToList();

                        chrTask.Values = values;
                    }
                }
                else
                {
                    chr.Missions.Add(chrMission = new Mission
                    {
                        MissionId = missionId,
                        State = MissionState.Active,
                        Tasks = missionTasks.Select(t =>
                        {
                            var values = new List<MissionTaskValue>();

                            if (t.TaskType == (int) MissionTaskType.ObtainItem)
                                values = t.Targets
                                    .Where(tgt => tgt is int && chr.Items.Exists(itm => itm.LOT == (int) tgt))
                                    .Select(tgt => new MissionTaskValue {Value = (float) (int) tgt}).ToList();

                            return new MissionTask
                            {
                                TaskId = t.UId,
                                Values = values
                            };
                        }).ToList()
                    });
                }

                await ctx.SaveChangesAsync();

                NotifyMission(missionId, chrMission.State);
                SetMissionTypeState(mission.Type, mission.Subtype, MissionLockState.New);
            }
        }

        public void NotifyMission(int missionId, MissionState state, bool sendingRewards = false)
        {
            _world.Server.Send(new NotifyMissionMessage
            {
                ObjectId = ObjectId,
                MissionId = missionId,
                MissionState = state,
                SendingRewards = sendingRewards
            }, Endpoint);
        }

        public void SetMissionTypeState(string type, string subType, MissionLockState state)
        {
            _world.Server.Send(new SetMissionTypeStateMessage
            {
                ObjectId = ObjectId,
                LockState = state,
                Type = type,
                Subtype = subType
            }, Endpoint);
        }

        public async Task UpdateMissionTaskAsync(int value, MissionTaskType type = MissionTaskType.None, long collectibleId = -1)
        {
            var tasks = await _world.Server.CDClient.GetMissionTasksWithTargetAsync(value);

            using (var ctx = new UchuContext())
            {
                var chr = await ctx.Characters.Include(c => c.Missions).ThenInclude(c => c.Tasks)
                    .SingleAsync(c => c.CharacterId == ObjectId);

                foreach (var task in tasks)
                {
                    if (type != MissionTaskType.None && task.TaskType != (int) type ||
                        task.Targets.All(t => t != (object) value) ||
                        chr.Missions.SelectMany(m => m.Tasks).All(t => t.TaskId != task.UId))
                        continue;

                    var chrMission = chr.Missions.Find(m => m.Tasks.Exists(t => t.TaskId == task.UId));
                    var chrTask = chrMission.Tasks.Find(t => t.TaskId == task.UId);

                    chrTask.Values.Add(new MissionTaskValue {Value = value});

                    float update = chrTask.Values.Count;

                    if (task.TaskType == (int) MissionTaskType.Collect)
                        update = collectibleId + (_world.ZoneInfo.ZoneId << 8);

                    NotifyMissionTask(chrMission.MissionId, chrMission.Tasks.IndexOf(chrTask), update);
                }

                await ctx.SaveChangesAsync();
            }
        }

        public void NotifyMissionTask(int missionId, int taskIndex, float update)
            => NotifyMissionTask(missionId, taskIndex, new[] {update});

        public void NotifyMissionTask(int missionId, int taskIndex, float[] updates)
        {
            _world.Server.Send(new NotifyMissionTaskMessage
            {
                ObjectId = ObjectId,
                MissionId = missionId,
                TaskIndex = 1 << (taskIndex + 1),
                Updates = updates
            }, Endpoint);
        }

        public async Task CompleteMissionTaskAsync(int missionId, int index)
        {
            var missionTasks = await _world.Server.CDClient.GetMissionTasksAsync(missionId);

            using (var ctx = new UchuContext())
            {
                var chr = await ctx.Characters.Include(c => c.Missions).ThenInclude(c => c.Tasks)
                    .SingleAsync(c => c.CharacterId == ObjectId);

                if (!chr.Missions.Exists(m => m.MissionId == missionId))
                {
                    await GiveMissionAsync(missionId);

                    /*chr.Missions.Add(new Mission
                    {
                        MissionId = missionId,
                        State = (int) MissionState.Active,
                        Tasks = missionTasks.Select(t =>
                        {
                            var values = new List<float>();

                            if (t.TaskType == (int) MissionTaskType.ObtainItem)
                                values = t.Targets.Where(tgt => chr.Items.Exists(itm => (object) itm.LOT == tgt))
                                    .Select(tgt => (float) tgt).ToList();

                            return new MissionTask
                            {
                                TaskId = t.UId,
                                Values = values
                            };
                        }).ToList()
                    });*/
                }

                var task = missionTasks[index];
                var chrMission = chr.Missions.Find(m => m.MissionId == missionId);

                chrMission.Tasks[index].Values = new List<MissionTaskValue>(task.TargetValue);

                await ctx.SaveChangesAsync();
            }
        }

        public async Task CompleteMissionAsync(int missionId)
        {
            var missionTasks = await _world.Server.CDClient.GetMissionTasksAsync(missionId);

            using (var ctx = new UchuContext())
            {
                var chr = await ctx.Characters.Include(c => c.Missions).ThenInclude(c => c.Tasks)
                    .SingleAsync(c => c.CharacterId == ObjectId);

                if (!chr.Missions.Exists(m => m.MissionId == missionId))
                {
                    await GiveMissionAsync(missionId);

                    /*chr.Missions.Add(new Mission
                    {
                        MissionId = missionId,
                        State = (int) MissionState.Completed,
                        Tasks = missionTasks.Select(t =>
                        {
                            var values = new List<float>();

                            if (t.TaskType == (int) MissionTaskType.ObtainItem)
                                values = t.Targets.Where(tgt => chr.Items.Exists(itm => (object) itm.LOT == tgt))
                                    .Select(tgt => (float) tgt).ToList();

                            return new MissionTask
                            {
                                TaskId = t.UId,
                                Values = values
                            };
                        }).ToList()
                    });*/
                }

                var chrMission = chr.Missions.Find(m => m.MissionId == missionId);

                foreach (var chrMisTask in chrMission.Tasks)
                {
                    var task = missionTasks.First(t => t.UId == chrMisTask.TaskId);

                    if (task.TargetValue > chrMisTask.Values.Count)
                    {
                        var count = task.TargetValue - chrMisTask.Values.Count;

                        for (var i = 0; i < count; i++)
                            chrMisTask.Values.Add(new MissionTaskValue {Value = -1});
                    }
                }

                chrMission.State = MissionState.Completed;

                await ctx.SaveChangesAsync();

                NotifyMission(missionId, MissionState.Unavailable, true);

                await GiveMissionRewardsAsync(missionId);

                NotifyMission(missionId, MissionState.Completed);

                await UpdateMissionTaskAsync(missionId, MissionTaskType.MissionComplete);
            }
        }

        public async Task GiveMissionRewardsAsync(int missionId)
        {
            var mission = await _world.Server.CDClient.GetMissionAsync(missionId);

            if (mission.FirstItemReward != -1)
                await GiveItemAsync(mission.FirstItemReward, mission.FirstItemRewardCount);

            if (mission.SecondItemReward != -1)
                await GiveItemAsync(mission.SecondItemReward, mission.SecondItemRewardCount);

            if (mission.ThirdItemReward != -1)
                await GiveItemAsync(mission.ThirdItemReward, mission.ThirdItemRewardCount);

            if (mission.FourthItemReward != -1)
                await GiveItemAsync(mission.FourthItemReward, mission.FourthItemRewardCount);

            if (mission.LegoScoreReward > 0)
                await GiveLegoScore(mission.LegoScoreReward);

            if (mission.CurrencyReward > 0)
                await GiveCurrency(mission.CurrencyReward);
        }

        public async Task GiveLegoScore(int legoScore)
        {
            using (var ctx = new UchuContext())
            {
                var chr = await ctx.Characters.FindAsync(ObjectId);

                chr.UniverseScore += legoScore;

                await ctx.SaveChangesAsync();

                _world.Server.Send(new ModifyLegoScoreMessage
                {
                    Score = legoScore,
                    SourceType = 2
                }, Endpoint);
            }
        }

        public async Task SetCurrency(long currency)
        {
            using (var ctx = new UchuContext())
            {
                var chr = await ctx.Characters.FindAsync(ObjectId);

                chr.Currency = currency;

                await ctx.SaveChangesAsync();

                _world.Server.Send(new SetCurrencyMessage
                {
                    ObjectId = ObjectId,
                    Currency = chr.Currency,
                    Position = Vector3.Zero
                }, Endpoint);
            }
        }

        public async Task GiveCurrency(long currency)
        {
            using (var ctx = new UchuContext())
            {
                var chr = await ctx.Characters.FindAsync(ObjectId);

                chr.Currency += currency;

                await ctx.SaveChangesAsync();

                _world.Server.Send(new SetCurrencyMessage
                {
                    ObjectId = ObjectId,
                    Currency = chr.Currency,
                    Position = Vector3.Zero
                }, Endpoint);
            }
        }

        public async Task SetMaxImaginationAsync(int imagination)
        {
            using (var ctx = new UchuContext())
            {
                var chr = await ctx.Characters.FindAsync(ObjectId);

                chr.MaximumImagination = imagination;

                await ctx.SaveChangesAsync();

                _world.Server.Send(new UIMessageToClientMessage<Dictionary<string, string>>
                {
                    ObjectId = ObjectId,
                    Arguments = new AMF3<Dictionary<string, string>>(new Dictionary<string, string>
                    {
                        ["type"] = "imagination",
                        ["amount"] = imagination.ToString()
                    }),
                    MessageName = "MaxPlayerBarUpdate"
                }, Endpoint);

                UpdateStats(chr);
            }
        }

        public async Task SetImaginationAsync(int imagination)
        {
            using (var ctx = new UchuContext())
            {
                var chr = await ctx.Characters.FindAsync(ObjectId);

                chr.CurrentImagination = imagination;

                await ctx.SaveChangesAsync();

                UpdateStats(chr);
            }
        }

        public async Task GiveImaginationAsync(int imagination)
        {
            using (var ctx = new UchuContext())
            {
                var chr = await ctx.Characters.FindAsync(ObjectId);

                chr.CurrentImagination += imagination;

                await ctx.SaveChangesAsync();

                UpdateStats(chr);
            }
        }

        public async Task SetMaxHealthAsync(int health)
        {
            using (var ctx = new UchuContext())
            {
                var chr = await ctx.Characters.FindAsync(ObjectId);

                chr.MaximumHealth = health;

                await ctx.SaveChangesAsync();

                _world.Server.Send(new UIMessageToClientMessage<Dictionary<string, string>>
                {
                    ObjectId = ObjectId,
                    Arguments = new AMF3<Dictionary<string, string>>(new Dictionary<string, string>
                    {
                        ["type"] = "health",
                        ["amount"] = health.ToString()
                    }),
                    MessageName = "MaxPlayerBarUpdate"
                }, Endpoint);

                UpdateStats(chr);
            }
        }

        public async Task SetHealthAsync(int health)
        {
            using (var ctx = new UchuContext())
            {
                var chr = await ctx.Characters.FindAsync(Character.CharacterId);

                chr.CurrentHealth = health;

                await ctx.SaveChangesAsync();

                UpdateStats(chr);
            }
        }

        public async Task GiveHealthAsync(int health)
        {
            using (var ctx = new UchuContext())
            {
                var chr = await ctx.Characters.FindAsync(Character.CharacterId);

                chr.CurrentHealth += health;

                await ctx.SaveChangesAsync();

                UpdateStats(chr);
            }
        }

        public void UpdateStats(Character character)
        {
            Update(new StatsComponent
            {
                HasStats = true,
                CurrentArmor = (uint) character.CurrentArmor,
                MaxArmor = character.MaximumArmor,
                CurrentHealth = (uint) character.CurrentHealth,
                MaxHealth = character.MaximumHealth,
                CurrentImagination = (uint) character.CurrentImagination,
                MaxImagination = character.MaximumImagination,
            });
        }
    }
}