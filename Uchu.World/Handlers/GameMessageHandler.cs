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
        public async Task RespondToMission(RespondToMissionMessage msg, IPEndPoint endpoint)
        {
            var session = Server.SessionCache.GetSession(endpoint);

            using (var ctx = new UchuContext())
            {
                var character = await ctx.Characters.Include(c => c.Missions).ThenInclude(m => m.Tasks)
                    .SingleAsync(c => c.CharacterId == msg.PlayerObjectId);

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

                    return;
                }

                var mission = character.Missions.Find(m => m.MissionId == msg.MissionId);

                if (await AllTasksCompletedAsync(mission))
                {
                    mission.State = (int) MissionState.Completed;
                    mission.CompletionCount++;
                    mission.LastCompletion = DateTimeOffset.Now.ToUnixTimeSeconds();

                    await ctx.SaveChangesAsync();

                    await OfferMissionAsync(Server.Worlds[(ZoneId) session.ZoneId], msg.ReceiverObjectId,
                        msg.PlayerObjectId, endpoint);
                }
            }
        }

        [PacketHandler]
        public async Task MissionDialogueOk(MissionDialogueOkMessage msg, IPEndPoint endpoint)
        {
            var session = Server.SessionCache.GetSession(endpoint);

            using (var ctx = new UchuContext())
            {
                var character = await ctx.Characters.Include(c => c.Missions).ThenInclude(m => m.Tasks)
                    .SingleAsync(c => c.CharacterId == session.CharacterId);

                var mission = await Server.CDClient.GetMissionAsync(msg.MissionId);

                if (!character.Missions.Exists(m => m.MissionId == msg.MissionId))
                {
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

                var chrMission = character.Missions.Find(m => m.MissionId == msg.MissionId);

                if (!await AllTasksCompletedAsync(chrMission))
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

                if (mission.MaximumHealthReward > 0 || mission.MaximumImaginationReward > 0)
                {
                    var world = Server.Worlds[(ZoneId) session.ZoneId];
                    var obj = world.GetObject(session.CharacterId);

                    var comp = (StatsComponent) obj.Components.First(c => c is StatsComponent);

                    if (mission.MaximumHealthReward > 0)
                    {
                        character.CurrentHealth = character.MaximumHealth;

                        comp.MaxHealth = character.MaximumHealth;
                        comp.CurrentHealth = (uint) character.CurrentHealth;
                    }

                    if (mission.MaximumImaginationReward > 0)
                    {
                        character.CurrentImagination = character.MaximumImagination;

                        comp.MaxImagination = character.MaximumImagination;
                        comp.CurrentImagination = (uint) character.CurrentImagination;
                    }

                    var index = obj.Components.ToList().IndexOf(comp);

                    obj.Components[index] = comp;

                    world.ReplicaManager.SendSerialization(obj);
                }

                await ctx.SaveChangesAsync();

                Server.Send(new NotifyMissionMessage
                {
                    ObjectId = session.CharacterId,
                    MissionId = msg.MissionId,
                    MissionState = MissionState.Completed,
                    SendingRewards = false
                }, endpoint);
            }
        }

        [PacketHandler]
        public async Task ObjectCollected(HasBeenCollectedMessage msg, IPEndPoint endpoint)
        {
            var session = Server.SessionCache.GetSession(endpoint);
            var world = Server.Worlds[(ZoneId) session.ZoneId];

            await UpdateMissionTaskAsync(world, MissionTaskType.Collect, msg.ObjectId, msg.PlayerObjectId, endpoint);

            Server.Send(new HasBeenCollectedByClientMessage
            {
                ObjectId = msg.ObjectId,
                PlayerObjectId = msg.PlayerObjectId
            }, endpoint);
        }

        [PacketHandler]
        public async Task StartSkill(StartSkillMessage msg, IPEndPoint endpoint)
        {
            var stream = new BitStream(msg.Data);
            var behavior = await Server.CDClient.GetSkillBehaviorAsync((int) msg.SkillId);

            await HandleBehaviorAsync(stream, behavior.BehaviorId);

            Server.Send(new EchoStartSkillMessage
            {
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
            }, endpoint);
        }

        [PacketHandler]
        public void SyncSkill(SyncSkillMessage msg, IPEndPoint endpoint)
        {
            Server.Send(new EchoSyncSkillMessage
            {
                IsDone = msg.IsDone,
                Data = msg.Data,
                BehaviorHandle = msg.BehaviorHandle,
                SkillHandle = msg.SkillHandle
            }, endpoint);
        }

        public async Task HandleBehaviorAsync(BitStream stream, int behaviorId)
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

                    if (stream.ReadBit())
                    {
                        var success = await Server.CDClient.GetBehaviorParameterAsync(behaviorId, "on success");

                        await HandleBehaviorAsync(stream, (int) success.Value);
                    }
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

                            await HandleBehaviorAsync(stream, (int) action.Value);
                        }
                    }
                    else
                    {
                        var missAction = await Server.CDClient.GetBehaviorParameterAsync(behaviorId, "miss action");
                        var blockedAction =
                            await Server.CDClient.GetBehaviorParameterAsync(behaviorId, "blocked action");

                        if (blockedAction != null && stream.ReadBit())
                        {
                            await HandleBehaviorAsync(stream, (int) blockedAction.Value);
                        }
                        else
                        {
                            await HandleBehaviorAsync(stream, (int) missAction.Value);
                        }
                    }
                    break;
                }

                case BehaviorTemplateId.And:
                {
                    var parameters = await Server.CDClient.GetBehaviorParametersAsync(behaviorId);

                    foreach (var parameter in parameters)
                        await HandleBehaviorAsync(stream, (int) parameter.Value);
                    break;
                }

                case BehaviorTemplateId.ProjectileAttack:
                {
                    var target = stream.ReadLong();

                    Console.WriteLine($"Hit {target}");

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

                    await HandleBehaviorAsync(stream, (int) parameter.Value);

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

                        await HandleBehaviorAsync(stream, (int) action.Value);
                    }

                    break;
                }

                case BehaviorTemplateId.Stun:
                {
                    // TODO
                    break;
                }

                case BehaviorTemplateId.Knockback:
                {
                    stream.ReadBit();
                    break;
                }

                case BehaviorTemplateId.AttackDelay:
                {
                    var handle = stream.ReadUInt();
                    break;
                }

                case BehaviorTemplateId.Switch:
                {
                    var state = true;

                    if (await Server.CDClient.GetBehaviorParameterAsync(behaviorId, "isEnemyFaction") == null ||
                        (await Server.CDClient.GetBehaviorParameterAsync(behaviorId, "imagination")).Value > 0)
                        state = stream.ReadBit();

                    if (state)
                    {
                        var actionTrue = await Server.CDClient.GetBehaviorParameterAsync(behaviorId, "action_true");

                        await HandleBehaviorAsync(stream, (int) actionTrue.Value);
                    }
                    else
                    {
                        var actionFalse = await Server.CDClient.GetBehaviorParameterAsync(behaviorId, "action_false");

                        await HandleBehaviorAsync(stream, (int) actionFalse.Value);
                    }
                    break;
                }

                case BehaviorTemplateId.Chain:
                {
                    var index = stream.ReadUInt();

                    var behaviorParameter =
                        await Server.CDClient.GetBehaviorParameterAsync(behaviorId, $"behavior {index}");
                    var delayParameter = await Server.CDClient.GetBehaviorParameterAsync(behaviorId, "chain_delay");

                    await Task.Delay((int) delayParameter.Value); // is this right?

                    await HandleBehaviorAsync(stream, (int) behaviorParameter.Value);
                    break;
                }

                case BehaviorTemplateId.ForceMovement:
                {
                    // TODO
                    break;
                }

                case BehaviorTemplateId.Interrupt:
                {
                    // TODO
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

                    await HandleBehaviorAsync(stream, (int) behavior.Value);

                    break;
                }

                case BehaviorTemplateId.AirMovement:
                {
                    var handle = stream.ReadUInt();

                    // TODO: syncskill thing

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
            var missions = await Server.CDClient.GetNPCMissions((int) componentId);

            using (var ctx = new UchuContext())
            {
                var character = await ctx.Characters.Include(c => c.Missions).ThenInclude(m => m.Tasks)
                    .SingleAsync(c => c.CharacterId == characterId);

                foreach (var mission in missions)
                {
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