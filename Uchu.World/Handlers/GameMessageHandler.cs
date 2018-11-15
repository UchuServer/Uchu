using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RakDotNet;
using Uchu.Core;

namespace Uchu.World
{
    public class GameMessageHandler : HandlerGroupBase
    {
        private Dictionary<uint, int> _behaviors;

        public GameMessageHandler()
        {
            _behaviors = new Dictionary<uint, int>();
        }

        [PacketHandler]
        public async Task RequestUse(RequestUseMessage msg, IPEndPoint endpoint)
        {
            var session = Server.SessionCache.GetSession(endpoint);
            var world = Server.Worlds[(ZoneId) session.ZoneId];

            await UpdateMissionTaskAsync(world, MissionTaskType.Interact, msg.TargetObjectId, session.CharacterId,
                endpoint);

            if (msg.IsMultiInteract)
            {
                if (msg.MultiInteractType == 0) // mission
                {
                    Server.Send(new OfferMissionMessage
                    {
                        ObjectId = session.CharacterId,
                        MissionId = (int) msg.MultiInteractId,
                        OffererObjectId = msg.TargetObjectId
                    }, endpoint);

                    Server.Send(new OfferMissionMessage
                    {
                        ObjectId = session.CharacterId,
                        MissionId = (int) msg.MultiInteractId,
                        OffererObjectId = msg.TargetObjectId
                    }, endpoint);
                }
            }
            else
            {
                await OfferMissionAsync(world, msg.TargetObjectId, session.CharacterId, endpoint);
            }
        }

        [PacketHandler]
        public void LinkedMission(RequestLinkedMissionMessage msg, IPEndPoint endpoint)
        {
            Console.WriteLine($"Mission = {msg.MissionId}");
            Console.WriteLine($"Offered = {msg.OfferedMission}");
        }

        [PacketHandler]
        public async Task RespondToMission(RespondToMissionMessage msg, IPEndPoint endpoint)
        {
            var session = Server.SessionCache.GetSession(endpoint);

            using (var ctx = new UchuContext())
            {
                var character = await ctx.Characters.Include(c => c.Missions).ThenInclude(m => m.Tasks)
                    .SingleAsync(c => c.CharacterId == msg.PlayerObjectId);

                var mission = await Server.CDClient.GetMissionAsync(msg.MissionId);

                if (!character.Missions.Exists(m => m.MissionId == msg.MissionId))
                {
                    var tasks = await Server.CDClient.GetMissionTasksAsync(msg.MissionId);

                    character.Missions.Add(new Mission
                    {
                        MissionId = msg.MissionId,
                        Tasks = tasks.Select(t => new MissionTask
                        {
                            TaskId = t.UId,
                            Values = new List<float>()
                        }).ToList()
                    });

                    await ctx.SaveChangesAsync();

                    Server.Send(new NotifyMissionMessage
                    {
                        ObjectId = session.CharacterId,
                        MissionId = msg.MissionId,
                        MissionState = MissionState.Active
                    }, endpoint);

                    Server.Send(new SetMissionTypeStateMessage
                    {
                        ObjectId = session.CharacterId,
                        LockState = MissionLockState.New,
                        Subtype = mission.Subtype,
                        Type = mission.Type
                    }, endpoint);

                    return;
                }

                var charMission = character.Missions.Find(m => m.MissionId == msg.MissionId);

                if (!await AllTasksCompletedAsync(charMission))
                {
                    Server.Send(new NotifyMissionMessage
                    {
                        ObjectId = session.CharacterId,
                        MissionId = msg.MissionId,
                        MissionState = MissionState.Active
                    }, endpoint);

                    return;
                }

                Server.Send(new NotifyMissionMessage
                {
                    ObjectId = session.CharacterId,
                    MissionId = msg.MissionId,
                    MissionState = MissionState.Unavailable,
                    SendingRewards = true
                }, endpoint);

                charMission.State = (int) MissionState.Completed;
                charMission.CompletionCount++;
                charMission.LastCompletion = DateTimeOffset.Now.ToUnixTimeSeconds();

                character.Currency += mission.CurrencyReward;
                character.UniverseScore += mission.LegoScoreReward;
                character.MaximumHealth += mission.MaximumHealthReward;
                character.MaximumImagination += mission.MaximumImaginationReward;

                if (mission.CurrencyReward > 0)
                {
                    Server.Send(new SetCurrencyMessage
                    {
                        ObjectId = session.CharacterId,
                        Currency = character.Currency,
                        Position = Vector3.Zero // TODO: find out what to set this to
                    }, endpoint);
                }

                if (mission.LegoScoreReward > 0)
                {
                    Server.Send(new ModifyLegoScoreMessage
                    {
                        ObjectId = session.CharacterId,
                        Score = character.UniverseScore
                    }, endpoint);
                }

                if (mission.MaximumImaginationReward > 0)
                {
                    var dict = new Dictionary<string, object>
                    {
                        ["amount"] = character.MaximumImagination.ToString(),
                        ["type"] = "imagination"
                    };

                    Server.Send(new UIMessageToClientMessage
                    {
                        ObjectId = session.CharacterId,
                        Arguments = new AMF3<object>(dict),
                        MessageName = "MaxPlayerBarUpdate"
                    }, endpoint);
                }

                if (mission.MaximumHealthReward > 0)
                {
                    var dict = new Dictionary<string, object>
                    {
                        ["amount"] = character.MaximumHealth.ToString(),
                        ["type"] = "health"
                    };

                    Server.Send(new UIMessageToClientMessage
                    {
                        ObjectId = session.CharacterId,
                        Arguments = new AMF3<object>(dict),
                        MessageName = "MaxPlayerBarUpdate"
                    }, endpoint);
                }

                Server.Send(new NotifyMissionMessage
                {
                    ObjectId = session.CharacterId,
                    MissionId = msg.MissionId,
                    MissionState = MissionState.Completed,
                    SendingRewards = false
                }, endpoint);

                await ctx.SaveChangesAsync();

                await OfferMissionAsync(Server.Worlds[(ZoneId) session.ZoneId], msg.ReceiverObjectId,
                    msg.PlayerObjectId, endpoint);
            }
        }

        [PacketHandler]
        public async Task MissionDialogueOk(MissionDialogueOkMessage msg, IPEndPoint endpoint)
        {
            var session = Server.SessionCache.GetSession(endpoint);
            var world = Server.Worlds[(ZoneId) session.ZoneId];

            await OfferMissionAsync(world, msg.ObjectId, session.CharacterId, endpoint);
        }

        [PacketHandler]
        public async Task ObjectCollected(HasBeenCollectedMessage msg, IPEndPoint endpoint)
        {
            var session = Server.SessionCache.GetSession(endpoint);
            var world = Server.Worlds[(ZoneId) session.ZoneId];

            await UpdateMissionTaskAsync(world, MissionTaskType.Collect, msg.ObjectId, msg.PlayerObjectId, endpoint);

            /*Server.Send(new HasBeenCollectedByClientMessage
            {
                ObjectId = msg.ObjectId,
                PlayerObjectId = msg.PlayerObjectId
            }, endpoint);*/
        }

        [PacketHandler]
        public async Task StartSkill(StartSkillMessage msg, IPEndPoint endpoint)
        {
            /*Server.Send(new EchoStartSkillMessage
            {
                ObjectId = msg.ObjectId,
                IsMouseClick = msg.IsMouseClick,
                CasterLatency = msg.CasterLatency,
                CastType = msg.CastType,
                LastClickPosition = msg.LastClickPosition,
                OriginObjectId = msg.OriginObjectId,
                TargetObjectId = msg.TargetObjectId,
                OriginRotation = msg.OriginRotation,
                Data = msg.Data,
                SkillId = msg.SkillId,
                SkillHandle = msg.SkillHandle
            }, endpoint);*/

            var stream = new BitStream(msg.Data);
            var behavior = await Server.CDClient.GetSkillBehaviorAsync((int) msg.SkillId);

            await HandleBehaviorAsync(stream, behavior.BehaviorId, endpoint);
        }

        [PacketHandler]
        public async Task SyncSkill(SyncSkillMessage msg, IPEndPoint endpoint)
        {
            /*Server.Send(new EchoSyncSkillMessage
            {
                ObjectId = msg.ObjectId,
                IsDone = msg.IsDone,
                Data = msg.Data,
                BehaviorHandle = msg.BehaviorHandle,
                SkillHandle = msg.SkillHandle
            }, endpoint);*/

            Console.WriteLine($"Length = {msg.Data.Length}");
            Console.WriteLine($"BehaviorHandle = {msg.BehaviorHandle}");

            if (_behaviors.TryGetValue(msg.BehaviorHandle, out var id))
            {
                var stream = new BitStream(msg.Data);
                var template = await Server.CDClient.GetBehaviorTemplateAsync(id);

                switch ((BehaviorTemplateId) template.TemplateId)
                {
                    case BehaviorTemplateId.AttackDelay:
                    {
                        var action = await Server.CDClient.GetBehaviorParameterAsync(id, "action");
                        var delay = await Server.CDClient.GetBehaviorParameterAsync(id, "delay");

                        await Task.Delay((int) (delay.Value * 1000));

                        await HandleBehaviorAsync(stream, (int) action.Value, endpoint);

                        break;
                    }

                    case BehaviorTemplateId.ForceMovement:
                    case BehaviorTemplateId.AirMovement:
                    {
                        var behaviorId = stream.ReadUInt();
                        var target = stream.ReadULong();

                        Console.WriteLine($"BehaviorId = {behaviorId}");
                        Console.WriteLine($"Hit {target}");

                        if (target != 0)
                            await DropLootAsync((long) target, endpoint);

                        if (behaviorId != 0)
                            await HandleBehaviorAsync(stream, (int) behaviorId, endpoint);
                        break;
                    }
                }
            }
        }

        [PacketHandler]
        public async Task SetFlag(SetFlagMessage msg, IPEndPoint endpoint)
        {
            Console.WriteLine($"Flag = {msg.Flag}");
            Console.WriteLine($"FlagId = {msg.FlagId}");

            var session = Server.SessionCache.GetSession(endpoint);

            using (var ctx = new UchuContext())
            {
                var character = await ctx.Characters.Include(c => c.Missions).ThenInclude(m => m.Tasks)
                    .SingleAsync(c => c.CharacterId == session.CharacterId);

                foreach (var mission in character.Missions)
                {
                    if (mission.State != (int) MissionState.Active &&
                        mission.State != (int) MissionState.CompletedActive)
                        continue;

                    var tasks = await Server.CDClient.GetMissionTasksAsync(mission.MissionId);

                    var task = tasks.Find(t =>
                        t.TargetLOTs.Contains(msg.FlagId) &&
                        character.Missions.Exists(m => m.Tasks.Exists(a => a.TaskId == t.UId)));

                    if (task == null)
                        continue;

                    var charTask = mission.Tasks.Find(t => t.TaskId == task.UId);

                    if (!charTask.Values.Contains(msg.FlagId))
                        charTask.Values.Add(msg.FlagId);

                    Server.Send(new NotifyMissionTaskMessage
                    {
                        ObjectId = session.CharacterId,
                        MissionId = task.MissionId,
                        TaskIndex = tasks.IndexOf(task),
                        Updates = new[] {(float) charTask.Values.Count}
                    }, endpoint);

                    await ctx.SaveChangesAsync();
                }
            }
        }

        [PacketHandler]
        public async Task PickupItem(PickupItemMessage msg, IPEndPoint endpoint)
        {
            var session = Server.SessionCache.GetSession(endpoint);
            var world = Server.Worlds[(ZoneId) session.ZoneId];

            var itemLOT = world.GetLootLOT(msg.LootObjectId);

            using (var ctx = new UchuContext())
            {
                var character = await ctx.Characters.Include(c => c.Items)
                    .SingleAsync(c => c.CharacterId == session.CharacterId);

                var id = Utils.GenerateObjectId();
                var inventoryType =
                    await Server.CDClient.IsModelAsync(itemLOT) ? 5 :
                    await Server.CDClient.IsBrickAsync(itemLOT) ? 2 :
                    await Server.CDClient.IsItemAsync(itemLOT) ? 0 : 7;

                var items = character.Items.Where(i => i.InventoryType == inventoryType).ToArray();

                var slot = 0;

                if (items.Length > 0)
                {
                    var max = items.Max(i => i.Slot);
                    slot = max + 1;

                    for (var i = 0; i < max; i++)
                    {
                        if (items.All(itm => itm.Slot != i))
                            slot = i;
                    }
                }

                var item = new InventoryItem
                {
                    InventoryItemId = id,
                    LOT = itemLOT,
                    Slot = slot,
                    Count = 1,
                    InventoryType = inventoryType
                };

                character.Items.Add(item);

                await ctx.SaveChangesAsync();

                Server.Send(new AddItemToInventoryMessage
                {
                    ObjectId = session.CharacterId,
                    ItemLOT = itemLOT,
                    ItemCount = (uint) item.Count,
                    ItemObjectId = id,
                    Slot = item.Slot,
                    InventoryType = inventoryType
                }, endpoint);

                /*var comp = await Server.CDClient.GetItemComponent(itemLOT);

                Console.WriteLine($"Type = {comp.ItemType}");*/
            }
        }

        public async Task DropLootAsync(long objectId, IPEndPoint endpoint)
        {
            var session = Server.SessionCache.GetSession(endpoint);
            var world = Server.Worlds[(ZoneId) session.ZoneId];
            var obj = world.GetObject(objectId);
            var physics = (SimplePhysicsComponent) obj.Components.FirstOrDefault(c => c is SimplePhysicsComponent);

            if (physics == null)
                return;

            var rand = new Random();

            var spawnPosition = physics.Position;

            spawnPosition.Y++;

            var drops = await Server.CDClient.GetDropsForObjectAsync(obj.LOT);

            foreach (var drop in drops)
            {
                var count = rand.Next(drop.MinDrops, drop.MaxDrops);
                /*var items = (await Server.CDClient.GetItemDropsAsync(drop.LootTableIndex)).Where(i => !i.IsMissionDrop)
                    .ToArray();*/
                var items = await Server.CDClient.GetItemDropsAsync(drop.LootTableIndex);

                if (items.Length == 0)
                    return;

                for (var i = 0; i < count; i++)
                {
                    if (rand.NextDouble() <= drop.Percent)
                    {
                        var item = items[rand.Next(0, items.Length)];
                        var lootId = Utils.GenerateObjectId();
                        var finalPosition = physics.Position;

                        finalPosition.X += ((float) rand.NextDouble() % 1f - 0.5f) * 20f;
                        finalPosition.Z += ((float) rand.NextDouble() % 1f - 0.5f) * 20f;

                        world.RegisterLoot(lootId, item.ItemId);

                        Server.Send(new DropClientLootMessage
                        {
                            ObjectId = session.CharacterId,
                            UsePosition = true,
                            FinalPosition = finalPosition,
                            Currency = 0,
                            ItemLOT = item.ItemId,
                            LootObjectId = lootId,
                            OwnerObjectId = session.CharacterId,
                            SourceObjectId = objectId,
                            SpawnPosition = spawnPosition
                        }, endpoint);
                    }
                }
            }
        }

        public async Task HandleBehaviorAsync(BitStream stream, int behaviorId, IPEndPoint endpoint)
        {
            var template = await Server.CDClient.GetBehaviorTemplateAsync(behaviorId);

            switch ((BehaviorTemplateId) template.TemplateId)
            {
                case BehaviorTemplateId.BasicAttack:
                {
                    stream.AlignRead();

                    stream.ReadUShort();
                    stream.ReadBit();
                    stream.ReadBit();
                    stream.ReadBit();
                    stream.ReadUInt();

                    var damage = stream.ReadUInt();

                    Console.WriteLine($"Damage = {damage}");

                    stream.ReadBit();
                    break;
                }

                case BehaviorTemplateId.TacArc:
                {
                    var isHit = stream.ReadBit();

                    if (isHit)
                    {
                        if (await Server.CDClient.GetBehaviorParameterAsync(behaviorId, "check env") != null)
                            stream.ReadBit();

                        var targetCount = stream.ReadUInt();
                        var targets = new long[targetCount];
                        var action = await Server.CDClient.GetBehaviorParameterAsync(behaviorId, "action");

                        for (var i = 0; i < targetCount; i++)
                        {
                            targets[i] = stream.ReadLong();

                            Console.WriteLine($"Hit {targets[i]}");
                            await DropLootAsync(targets[i], endpoint);

                            await HandleBehaviorAsync(stream, (int) action.Value, endpoint);
                        }
                    }
                    else
                    {
                        var missAction = await Server.CDClient.GetBehaviorParameterAsync(behaviorId, "miss action");
                        var blockedAction =
                            await Server.CDClient.GetBehaviorParameterAsync(behaviorId, "blocked action");

                        if (blockedAction != null && stream.ReadBit())
                        {
                            await HandleBehaviorAsync(stream, (int) blockedAction.Value, endpoint);
                        }
                        else if (missAction != null)
                        {
                            await HandleBehaviorAsync(stream, (int) missAction.Value, endpoint);
                        }
                    }
                    break;
                }

                case BehaviorTemplateId.And:
                {
                    var parameters = await Server.CDClient.GetBehaviorParametersAsync(behaviorId);

                    foreach (var parameter in parameters)
                        await HandleBehaviorAsync(stream, (int) parameter.Value, endpoint);
                    break;
                }

                case BehaviorTemplateId.ProjectileAttack:
                {
                    var target = stream.ReadLong();

                    Console.WriteLine($"Hit {target}");
                    await DropLootAsync(target, endpoint);

                    var count = (await Server.CDClient.GetBehaviorParameterAsync(behaviorId, "spread count")).Value;
                    var projectiles = new long[(int) count];

                    for (var i = 0; i < count; i++)
                    {
                        projectiles[i] = stream.ReadLong();

                        Console.WriteLine($"Projectile {projectiles[i]}");
                    }

                    break;
                }

                case BehaviorTemplateId.MovementSwitch:
                {
                    var type = (MovementType) stream.ReadUInt();

                    var name =
                        type == MovementType.Ground ? "ground_action" :
                        type == MovementType.Jump ? "jump_action" :
                        type == MovementType.Falling ? "falling_action" :
                        type == MovementType.DoubleJump ? "double_jump_action" :
                        type == MovementType.Jetpack ? "jetpack_action" :
                        "";

                    var parameter = await Server.CDClient.GetBehaviorParameterAsync(behaviorId, name);

                    await HandleBehaviorAsync(stream, (int) parameter.Value, endpoint);

                    break;
                }

                case BehaviorTemplateId.AreaOfEffect:
                {
                    var targetCount = stream.ReadUInt();
                    var targets = new long[targetCount];
                    var action = await Server.CDClient.GetBehaviorParameterAsync(behaviorId, "action");

                    for (var i = 0; i < targetCount; i++)
                    {
                        targets[i] = stream.ReadLong();

                        Console.WriteLine($"Hit {targets[i]}");
                        await DropLootAsync(targets[i], endpoint);

                        await HandleBehaviorAsync(stream, (int) action.Value, endpoint);
                    }

                    break;
                }

                case BehaviorTemplateId.Stun:
                {
                    // TODO
                    Console.WriteLine("stun");
                    break;
                }

                case BehaviorTemplateId.Duration:
                {
                    var action = await Server.CDClient.GetBehaviorParameterAsync(behaviorId, "action");

                    await HandleBehaviorAsync(stream, (int) action.Value, endpoint);
                    break;
                }

                case BehaviorTemplateId.Knockback:
                {
                    stream.ReadBit();
                    Console.WriteLine("knockback");
                    break;
                }

                case BehaviorTemplateId.AttackDelay:
                {
                    var handle = stream.ReadUInt();

                    _behaviors[handle] = behaviorId;
                    break;
                }

                case BehaviorTemplateId.Switch:
                {
                    var handle = stream.ReadUInt();

                    Console.WriteLine("switch");

                    _behaviors[handle] = behaviorId;

                    /*var state = true;

                    if (await Server.CDClient.GetBehaviorParameterAsync(behaviorId, "isEnemyFaction") == null ||
                        (await Server.CDClient.GetBehaviorParameterAsync(behaviorId, "imagination")).Value > 0)
                        state = stream.ReadBit();

                    if (state)
                    {
                        var actionTrue = await Server.CDClient.GetBehaviorParameterAsync(behaviorId, "action_true");

                        await HandleBehaviorAsync(stream, originId, (int) actionTrue.Value);
                    }
                    else
                    {
                        var actionFalse = await Server.CDClient.GetBehaviorParameterAsync(behaviorId, "action_false");

                        await HandleBehaviorAsync(stream, originId, (int) actionFalse.Value);
                    }*/
                    break;
                }

                case BehaviorTemplateId.Chain:
                {
                    var index = stream.ReadUInt();

                    var behaviorParameter =
                        await Server.CDClient.GetBehaviorParameterAsync(behaviorId, $"behavior {index}");
                    var delayParameter = await Server.CDClient.GetBehaviorParameterAsync(behaviorId, "chain_delay");

                    // await Task.Delay((int) (delayParameter.Value * 1000)); // is this right?

                    await HandleBehaviorAsync(stream, (int) behaviorParameter.Value, endpoint);
                    break;
                }

                case BehaviorTemplateId.ForceMovement:
                {
                    var hitAction = await Server.CDClient.GetBehaviorParameterAsync(behaviorId, "hit_action");
                    var hitActionEnemy =
                        await Server.CDClient.GetBehaviorParameterAsync(behaviorId, "hit_action_enemy");
                    var hitActionFaction =
                        await Server.CDClient.GetBehaviorParameterAsync(behaviorId, "hit_action_faction");

                    if (hitAction != null && hitAction.Value > 0 ||
                        hitActionEnemy != null && hitActionEnemy.Value > 0 ||
                        hitActionFaction != null && hitActionFaction.Value > 0)
                    {
                        var handle = stream.ReadUInt();

                        _behaviors[handle] = behaviorId;
                    }

                    break;
                }

                case BehaviorTemplateId.Interrupt:
                {
                    // TODO
                    Console.WriteLine("interrupt");
                    break;
                }

                case BehaviorTemplateId.SwitchMultiple:
                {
                    var value = stream.ReadFloat();
                    var name =
                        value <= (await Server.CDClient.GetBehaviorParameterAsync(behaviorId, "value 1")).Value
                            ? "behavior 1"
                            : "behavior 2";

                    var behavior = await Server.CDClient.GetBehaviorParameterAsync(behaviorId, name);

                    await HandleBehaviorAsync(stream, (int) behavior.Value, endpoint);

                    break;
                }

                case BehaviorTemplateId.AirMovement:
                {
                    var handle = stream.ReadUInt();

                    _behaviors[handle] = behaviorId;

                    break;
                }

                default:
                {
                    Console.WriteLine($"Unhandled behavior: {template.TemplateId}");

                    break;
                }
            }
        }

        public async Task<bool> AllTasksCompletedAsync(Mission mission)
        {
            var tasks = await Server.CDClient.GetMissionTasksAsync(mission.MissionId);

            return tasks.TrueForAll(t => mission.Tasks.Find(t2 => t2.TaskId == t.UId).Values.Count >= t.TargetValue);
        }

        public async Task OfferMissionAsync(Core.World world, long objectId, long characterId, IPEndPoint endpoint)
        {
            var obj = world.GetObject(objectId);

            if (obj == null)
                return;

            var componentId = await Server.CDClient.GetComponentIdAsync(obj.LOT, 73);
            var missions = await Server.CDClient.GetNPCMissionsAsync((int) componentId);

            using (var ctx = new UchuContext())
            {
                var character = await ctx.Characters.Include(c => c.Missions).ThenInclude(m => m.Tasks)
                    .SingleAsync(c => c.CharacterId == characterId);

                foreach (var mission in missions)
                {
                    Console.WriteLine(mission.MissionId);

                    if (mission.AcceptsMission)
                    {
                        if (character.Missions.Exists(m => m.MissionId == mission.MissionId))
                        {
                            var charMission = character.Missions.Find(m => m.MissionId == mission.MissionId);

                            if (charMission.State != (int) MissionState.Completed && await AllTasksCompletedAsync(charMission))
                            {
                                Server.Send(new OfferMissionMessage
                                {
                                    ObjectId = character.CharacterId,
                                    MissionId = mission.MissionId,
                                    OffererObjectId = objectId
                                }, endpoint);

                                Server.Send(new OfferMissionMessage
                                {
                                    ObjectId = character.CharacterId,
                                    MissionId = mission.MissionId,
                                    OffererObjectId = objectId
                                }, endpoint);

                                break;
                            }
                        }
                    }

                    if (mission.OffersMission)
                    {
                        if (!character.Missions.Exists(m => m.MissionId == mission.MissionId) ||
                            character.Missions.Find(m => m.MissionId == mission.MissionId).State ==
                            (int) MissionState.Active ||
                            character.Missions.Find(m => m.MissionId == mission.MissionId).State ==
                            (int) MissionState.ReadyToComplete)
                        {
                            var miss = await Server.CDClient.GetMissionAsync(mission.MissionId);

                            var canOffer = true;

                            foreach (var id in miss.PrerequiredMissions)
                            {
                                if (id == 664)
                                    continue;

                                if (!character.Missions.Exists(m => m.MissionId == id))
                                {
                                    canOffer = false;
                                    break;
                                }

                                var chrMission = character.Missions.Find(m => m.MissionId == id);

                                if (!await AllTasksCompletedAsync(chrMission))
                                {
                                    canOffer = false;
                                    break;
                                }
                            }

                            if (!canOffer)
                                continue;

                            Server.Send(new OfferMissionMessage
                            {
                                ObjectId = character.CharacterId,
                                MissionId = mission.MissionId,
                                OffererObjectId = objectId
                            }, endpoint);

                            Server.Send(new OfferMissionMessage
                            {
                                ObjectId = character.CharacterId,
                                MissionId = mission.MissionId,
                                OffererObjectId = objectId
                            }, endpoint);

                            break;
                        }
                    }
                }
            }
        }

        public async Task UpdateMissionTaskAsync(Core.World world, MissionTaskType type, long objectId, long characterId,
            IPEndPoint endpoint)
        {
            var obj = world.GetObject(objectId);

            using (var ctx = new UchuContext())
            {
                var character = await ctx.Characters.Include(c => c.Missions).ThenInclude(t => t.Tasks)
                    .SingleAsync(c => c.CharacterId == characterId);

                foreach (var mission in character.Missions)
                {
                    if (mission.State != (int) MissionState.Active &&
                        mission.State != (int) MissionState.CompletedActive)
                        continue;

                    var tasks = await Server.CDClient.GetMissionTasksAsync(mission.MissionId);

                    var task = tasks.Find(t =>
                        t.TargetLOTs.Contains(obj.LOT) &&
                        character.Missions.Exists(m => m.Tasks.Exists(a => a.TaskId == t.UId)));

                    if (task == null)
                        continue;

                    var charTask = await ctx.MissionTasks.Include(t => t.Mission).ThenInclude(m => m.Character)
                        .SingleAsync(t => t.TaskId == task.UId && t.Mission.Character.CharacterId == characterId);

                    switch (type)
                    {
                        case MissionTaskType.Interact:
                            if (!charTask.Values.Contains(obj.LOT))
                                charTask.Values.Add(obj.LOT);

                            Server.Send(new NotifyMissionTaskMessage
                            {
                                ObjectId = characterId,
                                MissionId = task.MissionId,
                                TaskIndex = tasks.IndexOf(task),
                                Updates = new[] {(float) charTask.Values.Count}
                            }, endpoint);

                            await ctx.SaveChangesAsync();
                            break;
                        case MissionTaskType.Collect:
                            var component = (CollectibleComponent) obj.Components.First(c => c is CollectibleComponent);

                            if (!charTask.Values.Contains(component.CollectibleId))
                                charTask.Values.Add(component.CollectibleId);

                            Server.Send(new NotifyMissionTaskMessage
                            {
                                ObjectId = characterId,
                                MissionId = task.MissionId,
                                TaskIndex = tasks.IndexOf(task),
                                Updates = new[] {(float) (component.CollectibleId + (world.ZoneId << 8))}
                            }, endpoint);

                            await ctx.SaveChangesAsync();
                            break;
                    }
                }
            }
        }
    }
}