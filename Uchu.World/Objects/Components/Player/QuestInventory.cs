using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Uchu.Core;
using Uchu.Core.CdClient;
using Uchu.World.Parsers;

namespace Uchu.World
{
    [Essential]
    public class QuestInventory : Component
    {
        public Mission[] GetCompletedMissions()
        {
            using (var ctx = new UchuContext())
            {
                return ctx.Missions.Where(
                    m => m.Character.CharacterId == GameObject.ObjectId && m.State == (int) MissionState.Completed
                ).ToArray();
            }
        }
        
        public Mission[] GetActiveMissions()
        {
            using (var ctx = new UchuContext())
            {
                return ctx.Missions.Where(
                    m => m.Character.CharacterId == GameObject.ObjectId &&
                         m.State == (int) MissionState.Active ||
                         m.State == (int) MissionState.CompletedActive
                ).ToArray();
            }
        }
        
        public Mission[] GetMissions()
        {
            using (var ctx = new UchuContext())
            {
                return ctx.Missions.Where(
                    m => m.Character.CharacterId == GameObject.ObjectId
                ).ToArray();
            }
        }

        public void MessageOfferMission(int missionId, GameObject questGiver)
        {
            ((Player) GameObject).Message(new OfferMissionMessage
            {
                Associate = GameObject,
                MissionId = missionId,
                QuestGiver = questGiver
            });
        }

        public void MessageMissionState(int missionId, MissionState state, bool sendingRewards = false)
        {
            using (var ctx = new UchuContext())
            {
                var character = ctx.Characters.Include(c => c.Missions)
                    .Single(c => c.CharacterId == GameObject.ObjectId);

                var mission = character.Missions.Single(m => m.MissionId == missionId);
                
                mission.State = (int) state;

                ctx.SaveChanges();
            }
            ((Player) GameObject).Message(new NotifyMissionMessage
            {
                Associate = GameObject,
                MissionId = missionId,
                MissionState = state,
                SendingRewards = sendingRewards
            });
        }

        public void MessageMissionTypeState(MissionLockState state, string subType, string type)
        {
            ((Player) GameObject).Message(new SetMissionTypeStateMessage
            {
                Associate = GameObject,
                LockState = state,
                SubType = subType,
                Type = type
            });
        }

        public void MessageUpdateMissionTask(int missionId, int taskIndex, float[] updates)
        {
            ((Player) GameObject).Message(new NotifyMissionTaskMessage
            {
                Associate = GameObject,
                MissionId = missionId,
                TaskIndex = taskIndex,
                Updates = updates
            });
        }

        public async Task RespondToMissionAsync(int missionId, GameObject questGiver)
        {
            using (var ctx = new UchuContext())
            using (var cdClient = new CdClientContext())
            {
                var character = await ctx.Characters
                    .Include(c => c.Items)
                    .Include(c => c.Missions)
                    .ThenInclude(m => m.Tasks)
                    .SingleAsync(c => c.CharacterId == GameObject.ObjectId);
                
                var mission = await cdClient.MissionsTable.FirstAsync(m => m.Id == missionId);

                if (!character.Missions.Exists(m => m.MissionId == missionId))
                {
                    var tasks = cdClient.MissionTasksTable.Where(t => t.Id == missionId);

                    character.Missions.Add(new Mission
                    {
                        MissionId = missionId,
                        Tasks = tasks.Select(t => GetTask(character, t)).ToList()
                    });

                    await ctx.SaveChangesAsync();

                    MessageMissionState(missionId, MissionState.Active);

                    MessageMissionTypeState(MissionLockState.New, mission.Definedsubtype, mission.Definedtype);
                    
                    return;
                }

                var charMissions = character.Missions.Find(m => m.MissionId == missionId);

                if (!await MissionParser.AllTasksCompletedAsync(charMissions))
                {
                    MessageMissionState(missionId, MissionState.Active);
                    
                    return;
                }

                await CompleteMissionAsync(missionId);
                await questGiver.GetComponent<QuestGiverComponent>().OfferMissionAsync(GameObject as Player);
            }
        }

        public async Task CompleteMissionAsync(int missionId)
        {
            using (var ctx = new UchuContext())
            using (var cdClient = new CdClientContext())
            {
                var mission = await cdClient.MissionsTable.FirstAsync(m => m.Id == missionId);
                
                var character = await ctx.Characters
                    .Include(c => c.Items)
                    .Include(c => c.Missions)
                    .ThenInclude(m => m.Tasks)
                    .SingleAsync(c => c.CharacterId == GameObject.ObjectId);

                if (!character.Missions.Exists(m => m.MissionId == missionId))
                {
                    var tasks = cdClient.MissionTasksTable.Where(t => t.Id == missionId);
                    
                    character.Missions.Add(new Mission
                    {
                        MissionId = missionId,
                        State = (int) MissionState.Active,
                        Tasks = tasks.Select(t => GetTask(character, t)).ToList()
                    });
                }
                
                var charMissions = character.Missions.Find(m => m.MissionId == missionId);

                MessageMissionState(missionId, MissionState.Unavailable, true);

                charMissions.CompletionCount++;

                charMissions.LastCompletion = DateTimeOffset.Now.ToUnixTimeSeconds();

                if (character.MaximumImagination == 0 && mission.Rewardmaximagination > 0)
                {
                    await CompleteMissionAsync(664);
                }

                character.Currency += mission.Rewardcurrency ?? 0;
                character.UniverseScore += mission.LegoScore ?? 0;
                character.MaximumHealth += mission.Rewardmaxhealth ?? 0;
                character.MaximumImagination += mission.Rewardmaximagination ?? 0;

                MessageMissionState(missionId, MissionState.Completed);

                await ctx.SaveChangesAsync();
            }
        }

        public async Task UpdateObjectTask(MissionTaskType type, GameObject gameObject)
        {
            using (var ctx = new UchuContext())
            using (var cdClient = new CdClientContext())
            {
                var character = await ctx.Characters
                    .Include(c => c.Items)
                    .Include(c => c.Missions)
                    .ThenInclude(m => m.Tasks)
                    .SingleAsync(c => c.CharacterId == GameObject.ObjectId);

                foreach (var mission in character.Missions)
                {
                    if (mission.State != (int) MissionState.Active &&
                        mission.State != (int) MissionState.CompletedActive) continue;

                    var tasks = cdClient.MissionTasksTable.Where(t => t.Id == mission.MissionId).ToArray();

                    var task = tasks.FirstOrDefault(missionTask =>
                        MissionParser.GetTargets(missionTask).Contains(gameObject.Lot) &&
                        mission.Tasks.Exists(a => a.TaskId == missionTask.Uid));

                    if (task == null) continue;

                    var charTask = mission.Tasks.Find(t => t.TaskId == task.Uid);

                    switch (type)
                    {
                        case MissionTaskType.KillEnemy:
                            break;
                        case MissionTaskType.Script:
                            if (!charTask.Values.Contains(gameObject.Lot)) charTask.Values.Add(gameObject.Lot);
                            
                            MessageUpdateMissionTask((int) task.Id, tasks.IndexOf(task),
                                new[] {(float) charTask.Values.Count});

                            await ctx.SaveChangesAsync();
                            break;
                        case MissionTaskType.QuickBuild:
                            break;
                        case MissionTaskType.Collect:
                            var component = gameObject.GetComponent<CollectibleComponent>();

                            if (!charTask.Values.Contains(component.CollectibleId))
                                charTask.Values.Add(component.CollectibleId);

                            MessageUpdateMissionTask(
                                (int) task.Id, tasks.IndexOf(task),
                                new [] {(float) (component.CollectibleId + (gameObject.Zone.ZoneInfo.ZoneId << 8))}
                            );
                            
                            await ctx.SaveChangesAsync();
                            break;
                        case MissionTaskType.GoToNPC:
                            break;
                        case MissionTaskType.UseEmote:
                            break;
                        case MissionTaskType.UseConsumable:
                            break;
                        case MissionTaskType.UseSkill:
                            break;
                        case MissionTaskType.ObtainItem:
                            break;
                        case MissionTaskType.Discover:
                            break;
                        case MissionTaskType.None:
                            break;
                        case MissionTaskType.MinigameAchievement:
                            break;
                        case MissionTaskType.Interact:
                            if (!charTask.Values.Contains(gameObject.Lot)) charTask.Values.Add(gameObject.Lot);

                            MessageUpdateMissionTask((int) task.Id, tasks.IndexOf(task),
                                new[] {(float) charTask.Values.Count});

                            await ctx.SaveChangesAsync();
                            break;
                        case MissionTaskType.MissionComplete:
                            break;
                        case MissionTaskType.TamePet:
                            break;
                        case MissionTaskType.Racing:
                            break;
                        case MissionTaskType.Flag:
                            break;
                        case MissionTaskType.NexusTowerBrickDonation:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(type), type, null);
                    }
                    
                    if (!await MissionParser.AllTasksCompletedAsync(mission)) continue;

                    MessageMissionState(mission.MissionId, MissionState.ReadyToComplete);
                }

                var otherTasks = new List<MissionTasks>();

                foreach (var missionTask in cdClient.MissionTasksTable)
                {
                    if (MissionParser.GetTargets(missionTask).Contains(gameObject.Lot))
                        otherTasks.Add(missionTask);
                }

                foreach (var task in otherTasks)
                {
                    var mission = cdClient.MissionsTable.First(m => m.Id == task.Id);

                    if (mission.OfferobjectID != -1 || mission.TargetobjectID != -1 || (mission.IsMission ?? true) ||
                        task.TaskType != (int) type) continue;

                    var tasks = cdClient.MissionTasksTable.Where(m => m.Id == mission.Id).ToArray();

                    if (!character.Missions.Exists(m => m.MissionId == mission.Id))
                    {
                        if (!await MissionParser.CheckPrerequiredMissionsAsync(mission.PrereqMissionID,
                            GetCompletedMissions()))
                        {
                            continue;
                        }

                        character.Missions.Add(new Mission
                        {
                            MissionId = (int) mission.Id,
                            State = (int) MissionState.Active,
                            Tasks = tasks.Select(t => new MissionTask
                            {
                                TaskId = (int) t.Uid,
                                Values = new List<float>()
                            }).ToList()
                        });
                    }

                    var charMission = character.Missions.Find(m => m.MissionId == mission.Id);

                    if (charMission.State != (int) MissionState.Active ||
                        charMission.State != (int) MissionState.CompletedActive) continue;

                    var charTask = charMission.Tasks.Find(t => t.TaskId == task.Uid);

                    if (!charTask.Values.Contains(gameObject.Lot)) charTask.Values.Add(gameObject.Lot);

                    await ctx.SaveChangesAsync();

                    MessageUpdateMissionTask(charMission.MissionId, tasks.IndexOf(task),
                        new[] {(float) charTask.Values.Count});

                    if (await MissionParser.AllTasksCompletedAsync(charMission))
                        await CompleteMissionAsync(charMission.MissionId);
                }
            }
        }

        private static MissionTask GetTask(Character character, MissionTasks t)
        {
            var values = new List<float>();

            var targets = MissionParser.GetTargets(t);

            values.AddRange(targets
                .Where(lot => lot is int && character.Items.Exists(i => i.LOT == (int) lot))
                .Select(lot => (float) (int) lot));

            return new MissionTask
            {
                TaskId = t.Uid.Value,
                Values = values
            };
        }
    }
}