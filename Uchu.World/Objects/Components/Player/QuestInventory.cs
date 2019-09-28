using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Uchu.Core;
using Uchu.Core.CdClient;
using Uchu.World.Parsers;

namespace Uchu.World
{
    public class QuestInventory : Component
    {
        public Mission[] GetCompletedMissions()
        {
            using var ctx = new UchuContext();
            return ctx.Missions.Where(
                m => m.Character.CharacterId == GameObject.ObjectId && m.State == (int) MissionState.Completed
            ).ToArray();
        }

        public Mission[] GetActiveMissions()
        {
            using var ctx = new UchuContext();
            return ctx.Missions.Where(
                m => m.Character.CharacterId == GameObject.ObjectId &&
                     m.State == (int) MissionState.Active ||
                     m.State == (int) MissionState.CompletedActive
            ).ToArray();
        }

        public Mission[] GetMissions()
        {
            using var ctx = new UchuContext();
            return ctx.Missions.Where(
                m => m.Character.CharacterId == GameObject.ObjectId
            ).ToArray();
        }

        public void MessageOfferMission(int missionId, GameObject questGiver)
        {
            As<Player>().Message(new OfferMissionMessage
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

            As<Player>().Message(new NotifyMissionMessage
            {
                Associate = GameObject,
                MissionId = missionId,
                MissionState = state,
                SendingRewards = sendingRewards
            });
        }

        public void MessageMissionTypeState(MissionLockState state, string subType, string type)
        {
            As<Player>().Message(new SetMissionTypeStateMessage
            {
                Associate = GameObject,
                LockState = state,
                SubType = subType,
                Type = type
            });
        }

        public void MessageUpdateMissionTask(int missionId, int taskIndex, float[] updates)
        {
            As<Player>().Message(new NotifyMissionTaskMessage
            {
                Associate = GameObject,
                MissionId = missionId,
                TaskIndex = taskIndex,
                Updates = updates
            });
        }

        public async Task RespondToMissionAsync(int missionId, GameObject questGiver, Lot rewardItem)
        {
            Logger.Information($"Responding {missionId}");

            //
            // The player has clicked on the accept or complete button.
            //

            using var ctx = new UchuContext();
            using var cdClient = new CdClientContext();
            
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
            // Complete mission.
            //

            await CompleteMissionAsync(missionId, rewardItem);

            //
            // Offer any fallow up missions from the quest giver.
            //

            questGiver.GetComponent<QuestGiverComponent>().OfferMission(As<Player>());
        }

        public async Task CompleteMissionAsync(int missionId, Lot rewardItem = default)
        {
            Logger.Information($"Completing mission {missionId}");

            using var ctx = new UchuContext();
            using var cdClient = new CdClientContext();
            
            //
            // Get mission information.
            //
            
            var mission = await cdClient.MissionsTable.FirstOrDefaultAsync(m => m.Id == missionId);

            //
            // Get character information.
            //
            
            var character = await ctx.Characters
                .Include(c => c.Items)
                .Include(c => c.Missions)
                .ThenInclude(m => m.Tasks)
                .SingleAsync(c => c.CharacterId == GameObject.ObjectId);

            //
            // If this mission is not already accepted, accept it and move on to complete it.
            //
            
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

            //
            // Save changes to be able to update its state.
            //
            
            await ctx.SaveChangesAsync();
            
            MessageMissionState(missionId, MissionState.Unavailable, true);
            
            //
            // Get character mission to complete.
            //
            
            var characterMission = character.Missions.Find(m => m.MissionId == missionId);
            
            var repeat = characterMission.CompletionCount != 0;
            characterMission.CompletionCount++;
            characterMission.LastCompletion = DateTimeOffset.Now.ToUnixTimeSeconds();

            // TODO: Remove/Move
            if (character.MaximumImagination == 0 && mission.Rewardmaximagination > 0)
            {
                await CompleteMissionAsync(664);
            }

            //
            // Update player based on rewards.
            //
            
            As<Player>().Currency += mission.Rewardcurrency ?? 0;
            As<Player>().UniverseScore += mission.LegoScore ?? 0;

            character.MaximumHealth += mission.Rewardmaxhealth ?? 0;
            character.MaximumImagination += mission.Rewardmaximagination ?? 0;

            if (mission.Rewardmaximagination > 0)
            {
                //
                // Make the client understand it not got imagination.
                //
            
                character.CurrentImagination += mission.Rewardmaximagination ?? 0;

                GameObject.Serialize(GameObject);
            }
            
            //
            // Get item rewards.
            //

            Logger.Debug($"{GameObject} chose item: {rewardItem}");

            var inventory = GameObject.GetComponent<InventoryManager>();
            
            var rewards = new (Lot, int)[]
            {
                ((repeat ? mission.Rewarditem1repeatable : mission.Rewarditem1) ?? 0, 
                    (repeat ? mission.Rewarditem1repeatcount : mission.Rewarditem1count) ?? 1),
                
                ((repeat ? mission.Rewarditem2repeatable : mission.Rewarditem2) ?? 0, 
                    (repeat ? mission.Rewarditem2repeatcount : mission.Rewarditem2count) ?? 1),
                
                ((repeat ? mission.Rewarditem3repeatable : mission.Rewarditem3) ?? 0, 
                    (repeat ? mission.Rewarditem3repeatcount : mission.Rewarditem3count) ?? 1),
                
                ((repeat ? mission.Rewarditem4repeatable : mission.Rewarditem4) ?? 0, 
                    (repeat ? mission.Rewarditem4repeatcount : mission.Rewarditem4count) ?? 1),
            };

            if (rewardItem == -1)
            {
                foreach (var (lot, count) in rewards)
                {
                    Logger.Debug($"Reward: {lot} x {count}");
                    
                    if (lot == default || count == default) continue;
                    
                    await inventory.AddItemAsync(lot, (uint) count);
                }
            }
            else
            {
                var (lot, count) = rewards[rewardItem];
                
                if (lot != default && count != default)
                    await inventory.AddItemAsync(lot, (uint) count);
            }

            //
            // Inform the client it's now complete.
            //
            
            MessageMissionState(missionId, MissionState.Completed);

            await ctx.SaveChangesAsync();
        }

        public async Task UpdateObjectTaskAsync(MissionTaskType type, Lot lot, GameObject gameObject = default)
        {
            Logger.Information($"{type} {lot}");

            using var ctx = new UchuContext();
            using var cdClient = new CdClientContext();
            
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
                    MissionParser.GetTargets(missionTask).Contains(lot) &&
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
                        if (!characterTask.Values.Contains(lot))
                            characterTask.Values.Add(lot);

                        // Send update to client
                        MessageUpdateMissionTask(
                            taskId, tasks.IndexOf(task),
                            new[] {(float) characterTask.Values.Count}
                        );

                        break;
                    case MissionTaskType.QuickBuild:
                        break;
                    case MissionTaskType.Collect:
                        if (gameObject == default)
                        {
                            Logger.Error($"{type} is only valid when {nameof(gameObject)} != null");
                            return;
                        }

                        var component = gameObject.GetComponent<CollectibleComponent>();

                        // Start this task value array
                        /*
                            if (!characterTask.Values.Contains(component.CollectibleId))
                            {
                                Logger.Information($"{Player} collected {component.CollectibleId}");
                            }
                            */

                        if (task.TargetValue > characterTask.Values.Count)
                        {
                            Logger.Information($"{GameObject} collected {component.CollectibleId}");

                            characterTask.Values.Add(component.CollectibleId);
                        }

                        Logger.Information($"Has collected {characterTask.Values.Count}/{task.TargetValue}");

                        // Send update to client
                        MessageUpdateMissionTask(
                            taskId, tasks.IndexOf(task),
                            new[]
                            {
                                (float) (component.CollectibleId + (gameObject.Zone.ZoneInfo.ZoneId << 8))
                            }
                        );

                        break;
                    case MissionTaskType.Discover:
                        break;
                    case MissionTaskType.None:
                        break;
                    case MissionTaskType.GoToNpc:
                    case MissionTaskType.MinigameAchievement:
                    case MissionTaskType.UseEmote:
                    case MissionTaskType.UseConsumable:
                    case MissionTaskType.UseSkill:
                    case MissionTaskType.ObtainItem:
                    case MissionTaskType.Interact:
                    case MissionTaskType.Flag:
                        // Start this task value array
                        if (!characterTask.Values.Contains(lot))
                            characterTask.Values.Add(lot);

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
                if (MissionParser.GetTargets(missionTask).Contains(lot))
                    otherTasks.Add(missionTask);

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

                    var hasPrerequisites = MissionParser.CheckPrerequiredMissions(
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
                if (!characterTask.Values.Contains(lot)) characterTask.Values.Add(lot);

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

        private static MissionTask GetTask(Character character, MissionTasks task)
        {
            var values = new List<float>();

            var targets = MissionParser.GetTargets(task);

            values.AddRange(targets
                .Where(lot => character.Items.Exists(i => i.LOT == lot))
                .Select(lot => (float) (int) lot));

            Debug.Assert(task.Uid != null, "t.Uid != null");
            return new MissionTask
            {
                TaskId = task.Uid.Value,
                Values = values
            };
        }
    }
}