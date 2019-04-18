using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.EntityFrameworkCore;
using RakDotNet;
using Uchu.Core;
using Uchu.Core.Collections;
using Uchu.Core.Packets.Server.GameMessages;

namespace Uchu.World
{
    public class GameMessageHandler : HandlerGroupBase
    {
        private readonly Dictionary<uint, int> _behaviors;

        public GameMessageHandler()
        {
            _behaviors = new Dictionary<uint, int>();
        }

        [PacketHandler]
        public async Task RequestDie(RequestDieMessage msg, IPEndPoint endPoint)
        {
            /*
             * TODO: Make work.
             */
            
            var session = Server.SessionCache.GetSession(endPoint);
            var world = Server.Worlds[(ZoneId) session.ZoneId];
            var player = world.GetPlayer(session.CharacterId);

            await player.Smash();
        }
        
        [PacketHandler]
        public async Task RequestSmashPlayer(RequestSmashPlayerMessage msg, IPEndPoint endPoint)
        {
            /*
             * TODO: Make work.
             */
            
            var session = Server.SessionCache.GetSession(endPoint);
            var world = Server.Worlds[(ZoneId) session.ZoneId];
            var player = world.GetPlayer(session.CharacterId);

            await player.Smash();
        }

        [PacketHandler]
        public async Task MoveItemInInventory(MoveItemInInventoryMessage msg, IPEndPoint endPoint)
        {
            await Player.MoveItemAsync(msg.ObjectID, msg.Slot);
        }

        [PacketHandler]
        public async Task RemoveItemFromInventory(RemoveItemFromInventoryMessage msg, IPEndPoint endPoint)
        {
            if (!msg.Confirmed) return;
            
            var session = Server.SessionCache.GetSession(endPoint);
            var world = Server.Worlds[(ZoneId) session.ZoneId];
            var player = world.GetPlayer(session.CharacterId);

            await player.RemoveItemFromInventoryAsync(msg.ObjID, msg.StackCount);
        }

        [PacketHandler]
        public async Task ClientItemConsumed(ClientItemConsumedMessage msg, IPEndPoint endPoint)
        {
            var session = Server.SessionCache.GetSession(endPoint);
            var world = Server.Worlds[(ZoneId) session.ZoneId];
            var player = world.GetPlayer(session.CharacterId);
            
            await player.RemoveItemFromInventoryAsync(msg.Item, 1);
        }

        [PacketHandler]
        public void PlayerLoaded(PlayerLoadedMessage msg, IPEndPoint endPoint)
        {
            /*
             * Sends RestoreToPostLoadStatsMessage to the player, making imagination useable.
             * TODO: Look into this being sent earlier.
             */
            
            var session = Server.SessionCache.GetSession(endPoint);
            var world = Server.Worlds[(ZoneId) session.ZoneId];
            var player = world.GetPlayer(session.CharacterId);
            Server.Send(new RestoreToPostLoadStatsMessage {ObjectId = player.CharacterId}, endPoint);
        }
        
        [PacketHandler]
        public async Task RequestUse(RequestUseMessage msg, IPEndPoint endpoint)
        {
            var session = Server.SessionCache.GetSession(endpoint);
            var world = Server.Worlds[(ZoneId) session.ZoneId];
            var player = world.GetPlayer(session.CharacterId);

            var obj = world.Zone.Scenes.SelectMany(s => s.Objects.Where(o => (long) o.ObjectId == msg.TargetObjectId))
                .FirstOrDefault();

            if (obj != null)
            {
                var lot = obj.LOT;

                var components = await Server.CDClient.GetComponentsAsync(lot);

                if (components.Any(c => c.ComponentType == 67))
                    await player.LaunchRocket(msg.TargetObjectId);
            }

            var rep = world.GetObject(msg.TargetObjectId);

            /*
             * Call OnUse on GameScripts assigned to this object.
             */
            foreach (var script in world.Replicas.Where(r => r.ObjectId == rep.ObjectId)
                .SelectMany(c => c.GameScripts))
            {
                script.OnUse(player);
            }

            await player.UpdateObjectTaskAsync(MissionTaskType.Interact, msg.TargetObjectId);

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

                    /*Server.Send(new OfferMissionMessage
                    {
                        ObjectId = session.CharacterId,
                        MissionId = (int) msg.MultiInteractId,
                        OffererObjectId = msg.TargetObjectId
                    }, endpoint);*/
                }
            }
            else
            {
                await player.OfferMissionAsync(msg.TargetObjectId);
            }
        }

        [PacketHandler]
        public void RebuildCancel(RebuildCancelMessage msg, IPEndPoint endpoint)
        {
            var session = Server.SessionCache.GetSession(endpoint);
            var world = Server.Worlds[(ZoneId) session.ZoneId];

            /*
             * Call OnRebuildCanceled on GameScripts assigned to this object.
             */
            foreach (var script in world.Replicas.Where(r => r.ObjectId == msg.ObjectId)
                .SelectMany(c => c.GameScripts))
            {
                if (script.ObjectID == msg.ObjectId)
                    script.OnRebuildCanceled(world.GetPlayer(msg.PlayerObjectId));
            }
        }

        [PacketHandler]
        public async Task LinkedMission(RequestLinkedMissionMessage msg, IPEndPoint endpoint)
        {
            Console.WriteLine($"Mission = {msg.MissionId}");
            Console.WriteLine($"Offered = {msg.OfferedMission}");
            
            /*
             * I had something going here but forgot.
             */
            var session = Server.SessionCache.GetSession(endpoint);
            var world = Server.Worlds[(ZoneId) session.ZoneId];
            var player = world.GetPlayer(session.CharacterId);
            await player.OfferMissionAsync(msg.MissionId);
        }

        [PacketHandler]
        public async Task RespondToMission(RespondToMissionMessage msg, IPEndPoint endpoint)
        {
            var session = Server.SessionCache.GetSession(endpoint);
            var world = Server.Worlds[(ZoneId) session.ZoneId];
            var player = world.GetPlayer(session.CharacterId);

            using (var ctx = new UchuContext())
            {
                var character = await ctx.Characters.Include(c => c.Items).Include(c => c.Missions).ThenInclude(m => m.Tasks)
                    .SingleAsync(c => c.CharacterId == msg.PlayerObjectId);

                var mission = await Server.CDClient.GetMissionAsync(msg.MissionId);

                if (!character.Missions.Exists(m => m.MissionId == msg.MissionId))
                {
                    var tasks = await Server.CDClient.GetMissionTasksAsync(msg.MissionId);

                    character.Missions.Add(new Mission
                    {
                        MissionId = msg.MissionId,
                        Tasks = tasks.Select(t =>
                        {
                            var values = new List<float>();

                            values.AddRange(t.Targets
                                .Where(lot => lot is int && character.Items.Exists(i => i.LOT == (int) lot))
                                .Select(lot => (float) (int) lot));

                            return new MissionTask
                            {
                                TaskId = t.UId,
                                Values = values
                            };
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

                if (!await player.AllTasksCompletedAsync(charMission))
                {
                    Server.Send(new NotifyMissionMessage
                    {
                        ObjectId = session.CharacterId,
                        MissionId = msg.MissionId,
                        MissionState = MissionState.Active
                    }, endpoint);

                    return;
                }

                await player.CompleteMissionAsync(mission);
                await player.OfferMissionAsync(msg.ReceiverObjectId);
            }
        }

        [PacketHandler]
        public async Task FireEvent(FireServerEventMessage msg, IPEndPoint endpoint)
        {
            var session = Server.SessionCache.GetSession(endpoint);
            var world = Server.Worlds[(ZoneId) session.ZoneId];

            switch (msg.Arguments)
            {
                case "ZonePlayer":
                {
                    var zoneId = msg.ThirdParameter;

                    var obj = world.Zone.Scenes.SelectMany(s => s.Objects.Where(o => (long) o.ObjectId == msg.ObjectId))
                        .First();

                    var launchpad =
                        await Server.CDClient.GetLaunchpadComponent(
                            (int) await Server.CDClient.GetComponentIdAsync(obj.LOT, 67));

                    using (var ctx = new UchuContext())
                    {
                        var character = await ctx.Characters.FindAsync(session.CharacterId);

                        character.LastZone = zoneId;

                        await ctx.SaveChangesAsync();
                    }

                    Server.Send(new TransferToZoneMessage
                    {
                        ObjectId = session.CharacterId,
                        TransferAllowedCheck = true,
                        Spawnpoint = launchpad.TargetScene,
                        ZoneId = (ushort) zoneId
                    }, endpoint);

                    Server.Send(new TransferToZoneCheckedMessage
                    {
                        ObjectId = session.CharacterId,
                        Spawnpoint = launchpad.TargetScene,
                        ZoneId = (ushort) zoneId
                    }, endpoint);

                    Server.SessionCache.SetZone(endpoint, (ZoneId) zoneId);

                    var zone = await Server.ZoneParser.ParseAsync(ZoneParser.Zones[(ushort) zoneId]);

                    Server.Send(new WorldInfoPacket
                    {
                        ZoneId = (ZoneId) zoneId,
                        Instance = 0,
                        Clone = 0,
                        Checksum = Utils.GetChecksum((ZoneId) zoneId),
                        Position = zone.SpawnPosition
                    }, endpoint);

                    break;
                }
            }
        }

        [PacketHandler]
        public void StartBuilding(StartBuildingMessage msg, IPEndPoint endpoint)
        {
            var session = Server.SessionCache.GetSession(endpoint);
            var world = Server.Worlds[(ZoneId) session.ZoneId];
            var obj = world.GetObject(session.CharacterId);
            var comp = (ControllablePhysicsComponent) obj.Components.First(c => c is ControllablePhysicsComponent);

            Server.Send(new StartArrangingMessage
            {
                ObjectId = session.CharacterId,
                FirstTime = msg.FirstTime,
                BuildAreaId = msg.ObjectId,
                StartPosition = comp.Position,
                SourceBag = msg.SourceBag,
                SourceObjectId = msg.SourceObjectId,
                SourceLOT = msg.SourceLOT,
                SourceType = 8, // TODO: find out how to get this
                TargetObjectId = msg.TargetObjectId,
                TargetLOT = msg.TargetLOT,
                TargetPosition = msg.TargetPosition,
                TargetType = msg.TargetType
            }, endpoint);
        }

        [PacketHandler]
        public async Task ModularBuildFinish(ModularBuildFinishMessage msg, IPEndPoint endpoint)
        {
            var session = Server.SessionCache.GetSession(endpoint);
            var world = Server.Worlds[(ZoneId) session.ZoneId];
            var player = world.GetPlayer(session.CharacterId);

            foreach (var lot in msg.Modules)
            {
                await player.RemoveItemAsync(lot);
            }

            var ldf = new LegoDataDictionary
            {
                ["assemblyPartLOTs"] = LegoDataList.FromEnumerable(msg.Modules)
            };

            await player.AddItemAsync(6416, extraInfo: ldf); // is this always 6416?

            Server.Send(new FinishArrangingMessage {ObjectId = session.CharacterId}, endpoint);
        }

        [PacketHandler]
        public async Task EquipItem(EquipItemRequestMessage msg, IPEndPoint endpoint)
        {
            var session = Server.SessionCache.GetSession(endpoint);
            var world = Server.Worlds[(ZoneId) session.ZoneId];
            var player = world.GetPlayer(session.CharacterId);

            await player.EquipItemAsync(msg.ItemObjectId);
        }

        [PacketHandler]
        public async Task UnequipItem(UnequipItemRequestMessage msg, IPEndPoint endpoint)
        {
            var session = Server.SessionCache.GetSession(endpoint);
            var world = Server.Worlds[(ZoneId) session.ZoneId];
            var player = world.GetPlayer(session.CharacterId);

            await player.UnequipItemAsync(msg.ItemObjectId);

            if (msg.ReplacementItemObjectId != -1)
                await player.EquipItemAsync(msg.ReplacementItemObjectId);
        }

        [PacketHandler]
        public void DoneArranging(DoneArrangingMessage msg, IPEndPoint endpoint)
        {
        }

        [PacketHandler]
        public void SetBuildMode(SetBuildModeMessage msg, IPEndPoint endpoint)
        {
        }

        [PacketHandler]
        public void BuildModeSet(BuildModeSetMessage msg, IPEndPoint endpoint)
        {
        }

        [PacketHandler]
        public async Task MissionDialogueOk(MissionDialogueOkMessage msg, IPEndPoint endpoint)
        {
            var session = Server.SessionCache.GetSession(endpoint);
            var world = Server.Worlds[(ZoneId) session.ZoneId];
            var player = world.GetPlayer(session.CharacterId);

            await player.OfferMissionAsync(msg.ObjectId);
        }

        [PacketHandler]
        public async Task ObjectCollected(HasBeenCollectedMessage msg, IPEndPoint endpoint)
        {
            var session = Server.SessionCache.GetSession(endpoint);
            var world = Server.Worlds[(ZoneId) session.ZoneId];
            var player = world.GetPlayer(session.CharacterId);

            await player.UpdateObjectTaskAsync(MissionTaskType.Collect, msg.ObjectId);
        }

        [PacketHandler]
        public async Task SetFlag(SetFlagMessage msg, IPEndPoint endpoint)
        {
            Console.WriteLine($"Flag = {msg.Flag}");
            Console.WriteLine($"FlagId = {msg.FlagId}");

            var session = Server.SessionCache.GetSession(endpoint);
            var world = Server.Worlds[(ZoneId) session.ZoneId];
            var player = world.GetPlayer(session.CharacterId);

            await player.UpdateTaskAsync(msg.FlagId, MissionTaskType.Flag);

            Server.Send(new NotifyClientFlagChangeMessage
            {
                Flag = msg.Flag,
                FlagId = msg.FlagId
            }, endpoint);
        }

        [PacketHandler]
        public async Task PickupItem(PickupItemMessage msg, IPEndPoint endpoint)
        {
            var session = Server.SessionCache.GetSession(endpoint);
            var world = Server.Worlds[(ZoneId) session.ZoneId];
            var player = world.GetPlayer(session.CharacterId);
            var itemLOT = world.GetLootLOT(msg.LootObjectId);

            await player.AddItemAsync(itemLOT);
        }

        [PacketHandler]
        public async Task PickupCurrency(PickupCurrencyMessage msg, IPEndPoint endpoint)
        {
            /*
             * Update the player currency when coins are picked up.
             */
            
            var session = Server.SessionCache.GetSession(endpoint);
            var world = Server.Worlds[(ZoneId) session.ZoneId];
            var player = world.GetPlayer(session.CharacterId);
            
            using (var ctx = new UchuContext())
            {
                var character = ctx.Characters.First(c => c.CharacterId == player.CharacterId);
                character.Currency += msg.Currency;
                
                Server.Send(new SetCurrencyMessage
                {
                    Currency = character.Currency,
                    Position = msg.Position,
                    ObjectId = player.CharacterId
                }, endpoint);

                await ctx.SaveChangesAsync();
            }
        }

        [PacketHandler]
        public async Task StartSkill(StartSkillMessage msg, IPEndPoint endpoint)
        {
            var session = Server.SessionCache.GetSession(endpoint);
            var world = Server.Worlds[(ZoneId) session.ZoneId];
            var player = world.GetPlayer(session.CharacterId);

            var stream = new BitStream(msg.Data);
            var behavior = await Server.CDClient.GetSkillBehaviorAsync((int) msg.SkillId);

            await HandleBehaviorAsync(stream, behavior.BehaviorId, endpoint);
            await player.UpdateTaskAsync((int) msg.SkillId, MissionTaskType.UseSkill);
        }

        [PacketHandler]
        public async Task SyncSkill(SyncSkillMessage msg, IPEndPoint endpoint)
        {
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

        public async Task DropLootAsync(long objectId, IPEndPoint endpoint)
        {
            var session = Server.SessionCache.GetSession(endpoint);
            var world = Server.Worlds[(ZoneId) session.ZoneId];

            foreach (var script in world.Replicas.Where(r => r.ObjectId == objectId)
                .SelectMany(c => c.GameScripts))
            {
                await script.OnSmash(world.Players.First(p => p.EndPoint.Equals(endpoint)));
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
                        var param = await Server.CDClient.GetBehaviorParameterAsync(behaviorId, "check_env");

                        if (param.Value > 0)
                            stream.ReadBit();

                        var targetCount = stream.ReadUInt();
                        var targets = new long[targetCount];
                        var action = await Server.CDClient.GetBehaviorParameterAsync(behaviorId, "action");

                        for (var i = 0; i < targetCount; i++)
                        {
                            targets[i] = stream.ReadLong();

                            Console.WriteLine($"Hit {targets[i]}");
                            await DropLootAsync(targets[i], endpoint);
                        }

                        for (var i = 0; i < targetCount; i++)
                        {
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
                    }

                    for (var i = 0; i < targetCount; i++)
                    {
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
    }
}