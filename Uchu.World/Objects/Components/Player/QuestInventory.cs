using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            Player.Message(new OfferMissionMessage
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
                var character = ctx.Characters
                    .Include(c => c.Missions)
                    .Single(c => c.CharacterId == GameObject.ObjectId);

                var mission = character.Missions.Single(m => m.MissionId == missionId);
                
                mission.State = (int) state;

                ctx.SaveChanges();
            }

            if (state == MissionState.ReadyToComplete) state = MissionState.Active;
            
            Player.Message(new NotifyMissionMessage
            {
                Associate = GameObject,
                MissionId = missionId,
                MissionState = state,
                SendingRewards = sendingRewards
            });
        }

        public void MessageMissionTypeState(MissionLockState state, string subType, string type)
        {
            Player.Message(new SetMissionTypeStateMessage
            {
                Associate = GameObject,
                LockState = state,
                SubType = subType,
                Type = type
            });
        }

        public void MessageUpdateMissionTask(int missionId, int taskIndex, float[] updates)
        {
            Player.Message(new NotifyMissionTaskMessage
            {
                Associate = GameObject,
                MissionId = missionId,
                TaskIndex = taskIndex,
                Updates = updates
            });
        }

        public async Task RespondToMissionAsync(int missionId, GameObject questGiver)
        {
            //
            // The player has clicked on the accept or complete button.
            //
            
            using (var ctx = new UchuContext())
            using (var cdClient = new CdClientContext())
            {
                //
                // Collect character data.
                //
                
                var character = await ctx.Characters
                    .Include(c => c.Items)
                    .Include(c => c.Missions)
                    .ThenInclude(m => m.Tasks)
                    .SingleAsync(c => c.CharacterId == GameObject.ObjectId);
                
                //
                // Get the mission the player is responding to.
                //
                
                var mission = await cdClient.MissionsTable.FirstAsync(m => m.Id == missionId);

                //
                // Get the character mission to update, if present.
                //
                
                var characterMission = character.Missions.Find(m => m.MissionId == missionId);
                
                //
                // Check if the player is accepting a mission or responding to one.
                //
                
                if (characterMission == default)
                {
                    //
                    // Player is accepting a new mission.
                    //
                    
                    //
                    // Get all the tasks of this mission setup the new mission.
                    //
                    
                    var tasks = cdClient.MissionTasksTable.Where(t => t.Id == missionId);

                    //
                    // Setup new mission
                    //
                    
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

                //
                // Player is responding to an active mission.
                //
                
                if (!await MissionParser.AllTasksCompletedAsync(characterMission))
                {
                    //
                    // Mission is not complete.
                    //
                    
                    MessageMissionState(missionId, MissionState.Active);
                    
                    return;
                }

                //
                // Complete mission
                //
                
                await CompleteMissionAsync(missionId);
                
                //
                // Offer any fallow up missions from the quest giver.
                //
                
                questGiver.GetComponent<QuestGiverComponent>().OfferMissionAsync(Player);
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
            Logger.Information($"{type} {gameObject}");
            
            using (var ctx = new UchuContext())
            using (var cdClient = new CdClientContext())
            {
                //
                // Collect character data.
                //
                
                var character = await ctx.Characters
                    .Include(c => c.Items)
                    .Include(c => c.Missions)
                    .ThenInclude(m => m.Tasks)
                    .SingleAsync(c => c.CharacterId == GameObject.ObjectId);

                //
                // Check if this object has anything to do with any of the active missions.
                //
                
                foreach (var mission in character.Missions)
                {
                    //
                    // Only active missions should have tasks that can be completed, the rest can be skipped.
                    //
                    
                    var missionState = (MissionState) mission.State;
                    if (missionState != MissionState.Active && missionState != MissionState.CompletedActive) continue;

                    //
                    // Get all the tasks this mission operates on.
                    //

                    var tasks = cdClient.MissionTasksTable.Where(
                        t => t.Id == mission.MissionId
                    ).ToArray();

                    //
                    // Get the task, if any, that includes any requirements related to this object.
                    //
                    
                    var task = tasks.FirstOrDefault(missionTask =>
                        MissionParser.GetTargets(missionTask).Contains(gameObject.Lot) &&
                        mission.Tasks.Exists(a => a.TaskId == missionTask.Uid)
                    );

                    //
                    // If not, move on to the next mission.
                    //
                    
                    if (task == default) continue;

                    //
                    // Get the task on the character mission which will be updated.
                    //
                    
                    var characterTask = mission.Tasks.Find(t => t.TaskId == task.Uid);

                    // Get task id.
                    if (task.Id == default) return;
                    var taskId = task.Id.Value;
                    
                    switch (type)
                    {
                        case MissionTaskType.KillEnemy:
                            break;
                        case MissionTaskType.Script:
                            
                            // Start this task value array
                            if (!characterTask.Values.Contains(gameObject.Lot))
                                characterTask.Values.Add(gameObject.Lot);

                            // Send update to client
                            MessageUpdateMissionTask(
                                taskId, tasks.IndexOf(task),
                                new[] {(float) characterTask.Values.Count}
                            );

                            break;
                        case MissionTaskType.QuickBuild:
                            break;
                        case MissionTaskType.Collect:
                            var component = gameObject.GetComponent<CollectibleComponent>();

                            // Start this task value array
                            if (!characterTask.Values.Contains(component.CollectibleId))
                                characterTask.Values.Add(component.CollectibleId);

                            // Send update to client
                            MessageUpdateMissionTask(
                                taskId, tasks.IndexOf(task),
                                new[]
                                {
                                    (float) (component.CollectibleId + (gameObject.Zone.ZoneInfo.ZoneId << 8))
                                }
                            );
                            
                            break;
                        case MissionTaskType.GoToNpc:
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
                            // Start this task value array
                            if (!characterTask.Values.Contains(gameObject.Lot))
                                characterTask.Values.Add(gameObject.Lot);

                            // Send update to client
                            MessageUpdateMissionTask(
                                taskId, tasks.IndexOf(task),
                                new[] {(float) characterTask.Values.Count}
                            );
                            
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
                    
                    await ctx.SaveChangesAsync();
                    
                    //
                    // Check if this mission is complete.
                    //
                    
                    if (!await MissionParser.AllTasksCompletedAsync(mission)) continue;
                    
                    MessageMissionState(mission.MissionId, MissionState.ReadyToComplete);
                }
                
                //
                // Collect tasks which fits the requirements of this action.
                //

                var otherTasks = new List<MissionTasks>();

                // ReSharper disable once LoopCanBeConvertedToQuery
                foreach (var missionTask in cdClient.MissionTasksTable)
                {
                    if (MissionParser.GetTargets(missionTask).Contains(gameObject.Lot))
                        otherTasks.Add(missionTask);
                }

                foreach (var task in otherTasks)
                {
                    var mission = cdClient.MissionsTable.First(m => m.Id == task.Id);

                    //
                    // Check if mission is an achievement and has a task of the correct type.
                    //
                    
                    if (mission.OfferobjectID != -1 || 
                        mission.TargetobjectID != -1 || 
                        (mission.IsMission ?? true) ||
                        task.TaskType != (int) type) 
                        continue;

                    //
                    // Get all tasks for the mission connected to this task.
                    //
                    
                    var tasks = cdClient.MissionTasksTable.Where(m => m.Id == mission.Id).ToArray(); 
                    
                    //
                    // Get the mission on the character. If present.
                    //
                    
                    var characterMission = character.Missions.Find(m => m.MissionId == mission.Id);
                    
                    //
                    // Check if the player could passably start this achievement.
                    //
                    
                    if (characterMission == default)
                    {
                        //
                        // Check if player has the Prerequisites to start this achievement.
                        //
                        
                        var hasPrerequisites = MissionParser.CheckPrerequiredMissionsAsync(
                            mission.PrereqMissionID,
                            GetCompletedMissions()
                        );
                        
                        if (!hasPrerequisites) continue;

                        //
                        // Player can start achievement.
                        //

                        // Get Mission Id of new achievement.
                        if (mission.Id == default) continue;
                        var missionId = mission.Id.Value;
                        
                        //
                        // Setup new achievement.
                        //
                        
                        characterMission = new Mission
                        {
                            MissionId = missionId,
                            State = (int) MissionState.Active,
                            Tasks = tasks.Select(t =>
                            {
                                Debug.Assert(t.Uid != null, "t.Uid != null");
                                return new MissionTask
                                {
                                    TaskId = (int) t.Uid,
                                    Values = new List<float>()
                                };
                            }).ToList()
                        };
                        
                        //
                        // Add achievement to the database.
                        //
                        
                        character.Missions.Add(characterMission);
                        
                        await ctx.SaveChangesAsync();
                    }

                    //
                    // Check if the mission is active.
                    //

                    var state = (MissionState) characterMission.State;
                    if (state != MissionState.Active || state != MissionState.CompletedActive) continue;

                    //
                    // Get the task to be updated.
                    //
                    
                    var characterTask = characterMission.Tasks.Find(t => t.TaskId == task.Uid);
                    
                    // Start this task value array
                    if (!characterTask.Values.Contains(gameObject.Lot)) characterTask.Values.Add(gameObject.Lot);

                    await ctx.SaveChangesAsync();

                    //
                    // Notify the client of the new achievement
                    //
                    
                    MessageUpdateMissionTask(
                        characterMission.MissionId,
                        tasks.IndexOf(task),
                        new[] {(float) characterTask.Values.Count}
                    );

                    //
                    // Check if achievement is complete.
                    //
                    
                    if (await MissionParser.AllTasksCompletedAsync(characterMission))
                        await CompleteMissionAsync(characterMission.MissionId);
                }
            }
        }

        public async Task UpdateLotTaskAsync(Lot lot, MissionTaskType type)
        {
            using (var ctx = new UchuContext())
            using (var cdClient = new CdClientContext())
            {
                var character = await ctx.Characters
                    .Include(c => c.Missions)
                    .ThenInclude(t => t.Tasks)
                    .SingleAsync(c => c.CharacterId == GameObject.ObjectId);

                foreach (var mission in character.Missions.Where(mission =>
                    mission.State == (int) MissionState.Active ||
                    mission.State == (int) MissionState.CompletedActive))
                {
                    var tasks = cdClient.MissionTasksTable.Where(t => t.Id == mission.Id).ToArray();

                    var task = tasks.FirstOrDefault(t =>
                        MissionParser.GetTargets(t).Contains(lot) &&
                        mission.Tasks.Exists(a => a.TaskId == t.Uid)
                    );
                    
                    if (task == default) continue;

                    var characterTask = mission.Tasks.Find(t => t.TaskId == task.Uid);

                    if (!characterTask.Values.Contains(lot)) characterTask.Values.Add(lot);

                    Debug.Assert(task.Id != null, "task.Id != null");
                    MessageUpdateMissionTask(
                        (int) task.Id,
                        tasks.IndexOf(task),
                        new[] {(float) characterTask.Values.Count}
                    );

                    await ctx.SaveChangesAsync();
                }
                
                var otherTasks = new List<MissionTasks>();

                foreach (var missionTask in cdClient.MissionTasksTable)
                {
                    if (MissionParser.GetTargets(missionTask).Contains(lot))
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
                        if (!MissionParser.CheckPrerequiredMissionsAsync(mission.PrereqMissionID,
                            GetCompletedMissions()))
                        {
                            continue;
                        }

                        Debug.Assert(mission.Id != null, "mission.Id != null");
                        character.Missions.Add(new Mission
                        {
                            MissionId = (int) mission.Id,
                            State = (int) MissionState.Active,
                            Tasks = tasks.Select(t =>
                            {
                                Debug.Assert(t.Uid != null, "t.Uid != null");
                                return new MissionTask
                                {
                                    TaskId = (int) t.Uid,
                                    Values = new List<float>()
                                };
                            }).ToList()
                        });
                    }

                    var charMission = character.Missions.Find(m => m.MissionId == mission.Id);

                    if (charMission.State != (int) MissionState.Active ||
                        charMission.State != (int) MissionState.CompletedActive) continue;

                    var charTask = charMission.Tasks.Find(t => t.TaskId == task.Uid);

                    if (!charTask.Values.Contains(lot)) charTask.Values.Add(lot);

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
                .Where(lot => character.Items.Exists(i => i.LOT == lot))
                .Select(lot => (float) (int) lot));

            Debug.Assert(t.Uid != null, "t.Uid != null");
            return new MissionTask
            {
                TaskId = t.Uid.Value,
                Values = values
            };
        }
    }
}