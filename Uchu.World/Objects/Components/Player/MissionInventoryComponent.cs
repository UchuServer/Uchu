using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Uchu.Core;
using Uchu.Core.Client;
using Uchu.World.Client;

namespace Uchu.World
{
    public class MissionInventoryComponent : Component
    {
        private readonly object _lock = new object();

        public Mission[] GetCompletedMissions()
        {
            using var ctx = new UchuContext();
            return ctx.Missions.Include(m => m.Tasks).ThenInclude(m => m.Values).Where(
                m => m.Character.CharacterId == GameObject.ObjectId && m.CompletionCount > 0
            ).ToArray();
        }

        public Mission[] GetActiveMissions()
        {
            using var ctx = new UchuContext();
            return ctx.Missions.Include(m => m.Tasks).ThenInclude(m => m.Values).Where(
                m => m.Character.CharacterId == GameObject.ObjectId &&
                     m.State == (int) MissionState.Active ||
                     m.State == (int) MissionState.CompletedActive
            ).ToArray();
        }

        public Mission[] GetMissions()
        {
            using var ctx = new UchuContext();
            return ctx.Missions.Include(m => m.Tasks).ThenInclude(m => m.Values).Where(
                m => m.Character.CharacterId == GameObject.ObjectId
            ).ToArray();
        }

        public void MessageOfferMission(int missionId, GameObject missionGiver)
        {
            As<Player>().Message(new OfferMissionMessage
            {
                Associate = GameObject,
                MissionId = missionId,
                QuestGiver = missionGiver
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

        public async Task RespondToMissionAsync(int missionId, GameObject missionGiver, Lot rewardItem)
        {
            Logger.Information($"Responding {missionId}");

            //
            // The player has clicked on the accept or complete button.
            //

            await using var ctx = new UchuContext();
            await using var cdClient = new CdClientContext();

            //
            // Collect character data.
            //

            var character = await ctx.Characters
                .Include(c => c.Items)
                .Include(c => c.Missions)
                .ThenInclude(m => m.Tasks)
                .ThenInclude(m => m.Values)
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

                if (character.Missions.Any(m => m.Id == missionId)) return;

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

                MessageMissionState(missionId, (MissionState) characterMission.State);

                MessageOfferMission(missionId, missionGiver);

                return;
            }

            //
            // Complete mission.
            //

            await CompleteMissionAsync(missionId, rewardItem);

            missionGiver?.GetComponent<MissionGiverComponent>().OfferMission(GameObject as Player);
        }

        public async Task CompleteMissionAsync(int missionId, Lot rewardItem = default)
        {
            As<Player>().SendChatMessage($"COMPLETING: {missionId}!", PlayerChatChannel.Normal);
            
            Logger.Information($"Completing mission {missionId}");

            await using var ctx = new UchuContext();
            await using var cdClient = new CdClientContext();

            //
            // Get mission information.
            //

            var mission = await cdClient.MissionsTable.FirstOrDefaultAsync(m => m.Id == missionId);

            if (mission == default) return;

            //
            // Get character information.
            //

            var character = await ctx.Characters
                .Include(c => c.Items)
                .Include(c => c.Missions)
                .ThenInclude(m => m.Tasks)
                .ThenInclude(m => m.Values)
                .SingleAsync(c => c.CharacterId == GameObject.ObjectId);

            //
            // If this mission is not already accepted, accept it and move on to complete it.
            //

            if (character.Missions.All(m => m.MissionId != missionId))
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

            if (characterMission.State == (int) MissionState.Completed) return;
            
            var repeat = characterMission.CompletionCount != 0;
            characterMission.CompletionCount++;
            characterMission.LastCompletion = DateTimeOffset.Now.ToUnixTimeSeconds();
            characterMission.State = (int) MissionState.Completed;
            
            await ctx.SaveChangesAsync();

            As<Player>().SendChatMessage($"COMPLETE: {missionId}!", PlayerChatChannel.Normal);

            //
            // Inform the client it's now complete.
            //

            MessageMissionState(missionId, MissionState.Completed);


            //
            // Update player based on rewards.
            //

            if (mission.IsMission ?? true)
            {
                // Mission

                As<Player>().Currency += mission.Rewardcurrency ?? 0;

                As<Player>().UniverseScore += mission.LegoScore ?? 0;
            }
            else
            {
                //
                // Achievement
                //
                // These rewards have the be silent, as the client adds them itself.
                //

                character.Currency += mission.Rewardcurrency ?? 0;
                character.UniverseScore += mission.LegoScore ?? 0;

                //
                // The client adds currency rewards as an offset, in my testing. Therefore we
                // have to account for this offset.
                //

                As<Player>().HiddenCurrency += mission.Rewardcurrency ?? 0;

                await ctx.SaveChangesAsync();
            }

            var stats = GameObject.GetComponent<Stats>();

            await stats.BoostBaseHealth((uint) (mission.Rewardmaxhealth ?? 0));
            await stats.BoostBaseImagination((uint) (mission.Rewardmaximagination ?? 0));

            //
            // Get item rewards.
            //

            var inventory = GameObject.GetComponent<InventoryManagerComponent>();

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

            var emotes = new[]
            {
                mission.Rewardemote ?? -1,
                mission.Rewardemote2 ?? -1,
                mission.Rewardemote3 ?? -1,
                mission.Rewardemote4 ?? -1
            };

            foreach (var i in emotes.Where(e => e != -1))
            {
                await As<Player>().UnlockEmoteAsync(i);
            }

            As<Player>().SendChatMessage($"REWARD: {rewardItem}", PlayerChatChannel.Normal);

            var isMission = mission.IsMission ?? true;
            
            if (rewardItem <= 0)
            {
                foreach (var (rewardLot, rewardCount) in rewards)
                {
                    var lot = rewardLot;
                    var count = rewardCount;
                    
                    As<Player>().SendChatMessage($"Item: {lot} x {count}");
                    
                    if (lot == default || count == default) continue;

                    if (isMission)
                    {
                        Detach(async () =>
                        {
                            await inventory.AddItemAsync(lot, (uint) count);
                        });
                    }
                    else
                    {
                        Detach(async () =>
                        {
                            As<Player>().SendChatMessage($"Here comes the item {lot} x {count}", PlayerChatChannel.Normal);
                            
                            await inventory.AddItemAsync(lot, (uint) count);
                        });
                    }
                }
            }
            else
            {
                var (lot, count) = rewards.FirstOrDefault(l => l.Item1 == rewardItem);

                if (lot != default && count != default)
                {
                    Detach(async () =>
                    {
                        await inventory.AddItemAsync(lot, (uint) count);
                    });
                }
            }
            
            Detach(() =>
            {
                UpdateObjectTask(MissionTaskType.MissionComplete, missionId);
            });
        }

        private async Task UpdateObjectTaskInternal(MissionTaskType type, Lot lot, GameObject gameObject = default, int selectedMissionId = -1)
        {
            await using var ctx = new UchuContext();
            await using var cdClient = new CdClientContext();

            //
            // Collect character data.
            //

            var character = await ctx.Characters
                .Include(c => c.Items)
                .Include(c => c.Missions)
                .ThenInclude(m => m.Tasks)
                .ThenInclude(m => m.Values)
                .SingleAsync(c => c.CharacterId == GameObject.ObjectId);

            //
            // Check if this object has anything to do with any of the active missions.
            //

            foreach (var mission in character.Missions)
            {
                if (selectedMissionId != -1 && mission.MissionId != selectedMissionId) continue;
                
                //
                // Only active missions should have tasks that can be completed, the rest can be skipped.
                //

                var missionState = (MissionState) mission.State;
                if (missionState != MissionState.Active && missionState != MissionState.CompletedActive) continue;

                //
                // Get mission
                //
                var clientMission = await cdClient.MissionsTable.FirstAsync(m => m.Id == mission.MissionId);

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
                {
                    var targets = MissionParser.GetTargets(missionTask);

                    if (!targets.Contains(lot)) return false;

                    if (!mission.Tasks.Exists(a => a.TaskId == missionTask.Uid)) return false;
                    
                    if (missionTask?.TaskType == null) return false;
                    
                    if ((MissionTaskType) missionTask.TaskType == MissionTaskType.GoToNpc)
                    {
                        missionTask.TaskType = (int) MissionTaskType.Interact;
                    }
                    
                    As<Player>().SendChatMessage($"{(MissionTaskType) missionTask.TaskType} -> {type}");

                    return missionTask.TaskType == (int) type;
                });

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

                As<Player>().SendChatMessage($"Target value: {task.TargetValue}");

                switch (type)
                {
                    // Special
                    case MissionTaskType.Collect:
                        if (gameObject == default)
                        {
                            Logger.Error($"{type} is only valid when {nameof(gameObject)} != null");
                            return;
                        }

                        var component = gameObject.GetComponent<CollectibleComponent>();

                        // The collectibleId bitshifted by the zoneId, as that is how the client expects it later
                        var shiftedId = (float) component.CollectibleId +
                                        ((int) gameObject.Zone.ZoneId << 8);

                        if (!characterTask.Contains(shiftedId) && task.TargetValue > characterTask.ValueArray().Length)
                        {
                            Logger.Information($"{GameObject} collected {component.CollectibleId}");
                            characterTask.Add(shiftedId);
                        }

                        Logger.Information($"Has collected {characterTask.ValueArray().Length}/{task.TargetValue}");

                        // Send update to client
                        MessageUpdateMissionTask(
                            taskId, tasks.IndexOf(task),
                            new[]
                            {
                                shiftedId
                            }
                        );

                        break;
                    // Allows multiple
                    case MissionTaskType.KillEnemy:
                    case MissionTaskType.QuickBuild:
                    case MissionTaskType.NexusTowerBrickDonation:
                    case MissionTaskType.None:
                    case MissionTaskType.MinigameAchievement:
                    case MissionTaskType.UseConsumable:
                    case MissionTaskType.ObtainItem:
                    case MissionTaskType.TamePet:
                    case MissionTaskType.Racing:
                        // Start this task value array
                        if (task.TargetValue > characterTask.ValueArray().Length)
                        {
                            characterTask.Add(lot);
                        }

                        // Send update to client
                        MessageUpdateMissionTask(
                            taskId, tasks.IndexOf(task),
                            new[] {(float) characterTask.ValueArray().Length}
                        );

                        break;
                    // Single
                    case MissionTaskType.Flag:
                    case MissionTaskType.MissionComplete:
                    case MissionTaskType.Interact:
                    case MissionTaskType.Discover:
                    case MissionTaskType.UseSkill:
                    case MissionTaskType.UseEmote:
                    case MissionTaskType.GoToNpc:
                    case MissionTaskType.Script:
                        if (characterTask.ValueArray().All(v => !v.Equals(lot)))
                        {
                            characterTask.Add(lot);
                        }

                        // Send update to client
                        MessageUpdateMissionTask(
                            taskId, tasks.IndexOf(task),
                            new[] {(float) characterTask.ValueArray().Length}
                        );

                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(type), type, null);
                }

                await ctx.SaveChangesAsync();

                //
                // Check if this mission is complete.
                //

                if (!await MissionParser.AllTasksCompletedAsync(mission)) continue;

                if (clientMission.IsMission ?? false)
                {
                    MessageMissionState(mission.MissionId, MissionState.ReadyToComplete);
                }
                else
                {
                    Detach(async () =>
                    {
                        await CompleteMissionAsync(mission.MissionId);
                    });
                }
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

                var characterMission = character.Missions.FirstOrDefault(m => m.MissionId == mission.Id);

                //
                // Check if the player could passably start this achievement.
                //

                if (characterMission != default) continue;

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
                    Tasks = tasks.Select(t => GetTask(character, t)).ToList()
                };

                //
                // Add achievement to the database.
                //

                character.Missions.Add(characterMission);

                await ctx.SaveChangesAsync();

                Detach(() =>
                {
                    UpdateObjectTask(type, lot, gameObject, missionId);
                });
            }
        }

        public void UpdateObjectTask(MissionTaskType type, Lot lot, GameObject gameObject = default, int selectedMissionId = -1)
        {
            // pls optimize this
            lock (this)
            {
                var task = UpdateObjectTaskInternal(type, lot, gameObject, selectedMissionId);

                task.Wait();
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
                Values = values.Select(v => new MissionTaskValue
                {
                    Value = v,
                    Count = 1
                }).ToList()
            };
        }
    }
}