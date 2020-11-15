using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Uchu.Core;
using Uchu.Core.Client;
using Uchu.World.Client;
using Uchu.World.Systems.Missions;

namespace Uchu.World
{
    public class MissionInventoryComponent : Component
    {
        public MissionInventoryComponent()
        {
            OnAcceptMission = new Event<MissionInstance>();
            OnCompleteMission = new Event<MissionInstance>();
            
            Listen(OnStart, async () =>
            {
                await LoadAsync();
            });
        }

        public Event<MissionInstance> OnAcceptMission { get; }
        
        public Event<MissionInstance> OnCompleteMission { get; }
        
        private List<MissionInstance> Missions { get; set; }
        
        public MissionInstance[] AllMissions
        {
            get
            {
                lock (Missions)
                {
                    return Missions.ToArray();
                }
            }
        }

        public MissionInstance[] CompletedMissions
        {
            get
            {
                lock (Missions)
                {
                    return Missions.Where(m => m.Completed).ToArray();
                }
            }
        }
        
        private async Task LoadAsync()
        {
            if (GameObject is Player player)
            {
                await using var cdContext = new CdClientContext();
                await using var uchuContext = new UchuContext();

                var missions = await uchuContext.Missions.Where(
                    m => m.CharacterId == GameObject.Id
                ).ToArrayAsync();

                Missions = new List<MissionInstance>();
                
                foreach (var mission in missions)
                {
                    var instance = new MissionInstance(player, mission.MissionId);
                    Missions.Add(instance);
                    await instance.LoadAsync(cdContext, uchuContext);
                }

                Listen(player.OnRespondToMission, async (missionId, receiver, rewardLot) =>
                {
                    await RespondToMissionAsync(missionId, receiver, rewardLot);
                });
            }
        }

        public bool HasActive(int id)
        {
            lock (Missions)
            {
                return Missions.Any(m => m.MissionId == id && m.State == MissionState.Active 
                                         || m.State == MissionState.CompletedActive);
            }
        }

        public bool HasCompleted(int id)
        {
            lock (Missions)
            {
                return Missions.Any(m => m.MissionId == id && m.State >= MissionState.Completed);
            }
        }

        public bool HasMission(int id)
        {
            lock (Missions)
            {
                return Missions.Any(m => m.MissionId == id);
            }
        }

        public MissionInstance GetMission(int id)
        {
            lock (Missions)
            {
                return Missions.FirstOrDefault(m => m.MissionId == id);
            }
        }

        public bool CanAccept(int id) => GetMission(id) is { } mission
                   && MissionParser.CheckPrerequiredMissions(mission.PrerequisiteMissions, CompletedMissions);

        public void MessageOfferMission(int missionId, GameObject missionGiver)
        {
            var player = (Player) GameObject;
            
            player.Message(new OfferMissionMessage
            {
                Associate = GameObject,
                MissionId = missionId,
                QuestGiver = missionGiver
            });
            
            player.Message(new OfferMissionMessage
            {
                Associate = missionGiver,
                MissionId = missionId,
                QuestGiver = missionGiver
            });
        }

        private async Task RespondToMissionAsync(int missionId, GameObject missionGiver, Lot rewardItem)
        {
            await using var uchuContext = new UchuContext();

            MissionInstance mission = GetMission(missionId);
            
            // If the user doesn't have this mission yet, start it
            if (mission == default)
            {
                await using var cdContext = new CdClientContext();

                var instance = new MissionInstance(GameObject as Player, missionId);
                await instance.LoadAsync(cdContext, uchuContext);
                
                lock (Missions) {
                    Missions.Add(instance);
                }
                
                return;
            }
            
            // Player is responding to an active mission.
            if (!mission.Completed)
            {
                MessageOfferMission(missionId, missionGiver);
                return;
            }
            
            // Complete mission
            await mission.CompleteAsync(uchuContext, rewardItem);
            missionGiver?.GetComponent<MissionGiverComponent>().HandleInteraction((Player)GameObject);
        }

        public async Task CompleteMissionAsync(int missionId)
        {
            MissionInstance mission;
            lock (Missions)
            {
                mission = Missions.FirstOrDefault(m => m.MissionId == missionId);
            }
            
            await using var uchuContext = new UchuContext();
            
            // If the player is completing a mission that hasn't started, start it first
            if (mission == default)
            {
                await using var cdContext = new CdClientContext();
                
                var instance = new MissionInstance((Player)GameObject, missionId);
                lock (Missions)
                {
                    Missions.Add(instance);
                }

                await instance.LoadAsync(cdContext, uchuContext);
                await instance.CompleteAsync(uchuContext);
                
                return;
            }

            await mission.CompleteAsync(uchuContext);
        }

        public async Task<T[]> FindActiveTasksAsync<T>() where T : MissionTaskInstance
        {
            var tasks = new List<T>();
            
            lock (Missions)
            { 
                foreach (var instance in Missions)
                {
                    if (instance.State != MissionState.Active && instance.State != MissionState.CompletedActive)
                        continue;

                    foreach (var task in instance.Tasks.OfType<T>())
                    {
                        if (task.Completed)
                            continue;
                        tasks.Add(task);
                    }
                }
            }

            return tasks.ToArray();
        }

        public async Task SmashAsync(Lot lot)
        {
            foreach (var task in await FindActiveTasksAsync<SmashTask>())
            {
                await task.ReportProgress(lot);
            }

            await SearchForNewAchievementsAsync<SmashTask>(MissionTaskType.Smash, lot, async task =>
            {
                await task.ReportProgress(lot);
            });
        }

        public async Task CollectAsync(GameObject gameObject)
        {
            foreach (var task in await FindActiveTasksAsync<CollectTask>())
            {
                await task.ReportProgress(gameObject);
            }

            await SearchForNewAchievementsAsync<CollectTask>(MissionTaskType.Collect, gameObject.Lot, async task =>
            {
                await task.ReportProgress(gameObject);
            });
        }

        public async Task ScriptAsync(int id)
        {
            foreach (var task in await FindActiveTasksAsync<ScriptTask>())
            {
                await task.ReportProgress(id);
            }

            await SearchForNewAchievementsAsync<ScriptTask>(MissionTaskType.Script, id, async task =>
            {
                await task.ReportProgress(id);
            });
        }

        public async Task QuickBuildAsync(Lot lot, int activity)
        {
            foreach (var task in await FindActiveTasksAsync<QuickBuildTask>())
            {
                await task.ReportProgress(lot, activity);
            }

            await SearchForNewAchievementsAsync<QuickBuildTask>(MissionTaskType.QuickBuild, lot, async task =>
            {
                await task.ReportProgress(lot, activity);
            });
        }

        public async Task GoToNpcAsync(Lot lot)
        {
            foreach (var task in await FindActiveTasksAsync<GoToNpcTask>())
            {
                await task.ReportProgress(lot);
            }
            
            await SearchForNewAchievementsAsync<GoToNpcTask>(MissionTaskType.GoToNpc, lot, async task =>
            {
                await task.ReportProgress(lot);
            });
        }
        
        public async Task InteractAsync(Lot lot)
        {
            foreach (var task in await FindActiveTasksAsync<InteractTask>())
            {
                await task.ReportProgress(lot);
            }
            
            await SearchForNewAchievementsAsync<InteractTask>(MissionTaskType.Interact, lot, async task =>
            {
                await task.ReportProgress(lot);
            });
        }

        public async Task UseEmoteAsync(GameObject gameObject, int emote)
        {
            foreach (var task in await FindActiveTasksAsync<UseEmoteTask>())
            {
                await task.ReportProgress(gameObject, emote);
            }

            await SearchForNewAchievementsAsync<UseEmoteTask>(MissionTaskType.UseEmote, emote, async task =>
            {
                await task.ReportProgress(gameObject, emote);
            });
        }

        public async Task UseConsumableAsync(Lot lot)
        {
            foreach (var task in await FindActiveTasksAsync<UseConsumableTask>())
            {
                await task.ReportProgress(lot);
            }

            await SearchForNewAchievementsAsync<UseConsumableTask>(MissionTaskType.UseConsumable, lot, async task =>
            {
                await task.ReportProgress(lot);
            });
        }

        public async Task UseSkillAsync(int skillId)
        {
            foreach (var task in await FindActiveTasksAsync<UseSkillTask>())
            {
                await task.ReportProgress(skillId);
            }

            await SearchForNewAchievementsAsync<UseSkillTask>(MissionTaskType.UseSkill, skillId, async task =>
            {
                await task.ReportProgress(skillId);
            });
        }

        public async Task ObtainItemAsync(Lot lot)
        {
            foreach (var task in await FindActiveTasksAsync<ObtainItemTask>())
            {
                await task.ReportProgress(lot);
            }

            await SearchForNewAchievementsAsync<ObtainItemTask>(MissionTaskType.ObtainItem, lot, async task =>
            {
                await task.ReportProgress(lot);
            });
        }

        public async Task MissionCompleteAsync(int id)
        {
            foreach (var task in await FindActiveTasksAsync<MissionCompleteTask>())
            {
                await task.ReportProgress(id);
            }

            await SearchForNewAchievementsAsync<MissionCompleteTask>(MissionTaskType.MissionComplete, id, async task =>
            {
                await task.ReportProgress(id);
            });
        }

        public async Task FlagAsync(int flag)
        {
            foreach (var task in await FindActiveTasksAsync<FlagTask>())
            {
                await task.ReportProgress(flag);
            }

            await SearchForNewAchievementsAsync<FlagTask>(MissionTaskType.Flag, flag, async task =>
            {
                await task.ReportProgress(flag);
            });
        }

        // TODO: Improve
        private async Task SearchForNewAchievementsAsync<T>(MissionTaskType type, Lot lot, Func<T, Task> progress = null) where T : MissionTaskInstance
        {
            await using var cdContext = new CdClientContext();
            await using var uchuContext = new UchuContext();
            
            // Collect tasks which fits the requirements of this action.
            var otherTasks = new List<MissionTasks>();

            foreach (var missionTask in ClientCache.Tasks)
                if (MissionParser.GetTargets(missionTask).Contains(lot))
                    otherTasks.Add(missionTask);

            foreach (var task in otherTasks)
            {
                var mission = await cdContext.MissionsTable.FirstOrDefaultAsync(m => m.Id == task.Id);
                if (mission == default)
                    continue;
                
                // Ensure that the mission is an achievement and has a task of the correct type.
                if (mission.OfferobjectID != -1 ||
                    mission.TargetobjectID != -1 ||
                    (mission.IsMission ?? true) ||
                    task.TaskType != (int) type)
                    continue;
                
                // Get the mission on the character. If present.
                MissionInstance characterMission;
                lock (Missions)
                {
                    characterMission = Missions.FirstOrDefault(m => m.MissionId == mission.Id);
                }
                
                // Check if the player could possibly start this achievement.
                if (characterMission != default)
                    continue;
                
                // Check if player has the Prerequisites to start this achievement.
                var hasPrerequisites = MissionParser.CheckPrerequiredMissions(
                    mission.PrereqMissionID,
                    CompletedMissions
                );

                if (!hasPrerequisites)
                    continue;
                
                // Player can start achievement.
                // Get Mission Id of new achievement.
                if (mission.Id == default)
                    continue;
                
                var missionId = mission.Id.Value;
                
                // Setup new achievement.
                var instance = new MissionInstance(GameObject as Player, missionId);
                
                lock (Missions)
                {
                    Missions.Add(instance);
                }
                
                await instance.LoadAsync(cdContext, uchuContext);

                var activeTask = instance.Tasks.First(t => t.TaskId == task.Uid);
                if (progress != null)
                {
                    var _ = Task.Run(async () => await progress(activeTask as T));
                }
            }
        }
    }
}