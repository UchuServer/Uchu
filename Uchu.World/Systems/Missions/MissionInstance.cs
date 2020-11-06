using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Uchu.Core;
using Uchu.Core.Client;

namespace Uchu.World.Systems.Missions
{
    public class MissionInstance
    {
        public Player Player { get; }
        
        // Template properties
        public int MissionId { get; }
        public bool IsMission { get; private set; }
        public bool IsRandom { get; private set; }
        public string DefinedType { get; private set; }
        public string DefinedSubType { get; private set; }
        public MissionTaskBase[] Tasks { get; private set; }
        
        // Instance properties
        public bool HasStarted { get; private set; }
        public MissionState State { get; private set; }
        
        private static Dictionary<MissionTaskType, Type> TaskTypes { get; }
        
        static MissionInstance()
        {
            TaskTypes = new Dictionary<MissionTaskType, Type>();
            
            var types = typeof(MissionInstance).Assembly.GetTypes().Where(
                t => t.BaseType == typeof(MissionTaskBase)
            );
            
            foreach (var type in types)
            {
                if (type.IsAbstract) continue;
                
                var instance = (MissionTaskBase) Activator.CreateInstance(type);

                try
                {
                    if (instance != null) TaskTypes[instance.Type] = type;
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }
        
        public MissionInstance(Player player, int missionId)
        {
            Player = player;
            MissionId = missionId;
        }

        /// <summary>
        /// Loads generic CdClient information about the mission.
        /// </summary>
        public async Task LoadMissionTemplate()
        {
            await using var cdClient = new CdClientContext();
            
            var mission = await cdClient.MissionsTable.FirstAsync(
                m => m.Id == MissionId
            );
            
            IsMission = mission.IsMission ?? true;
            IsRandom = mission.IsRandom ?? true;
            DefinedType = mission.Definedtype;
            DefinedSubType = mission.Definedsubtype;
        }
        
        /// <summary>
        /// Loads specific UchuContext information about the mission.
        /// </summary>
        private void LoadMissionInstance(Mission mission)
        {
            HasStarted = true;
            State = (MissionState)mission.State;
        }

        private async Task LoadMissionInstance()
        {
            await using var ctx = new UchuContext();

            // Mission instance information
            var mission = await ctx.Missions.FirstOrDefaultAsync(
                m => m.CharacterId == Player.Id && m.MissionId == MissionId
            );
            
            // Not yet started
            if (mission == default)
                return;

            LoadMissionInstance(mission);
        }

        public async Task LoadAsync()
        {
            await using var cdClient = new CdClientContext();

            await LoadMissionTemplate();
            await LoadMissionInstance();

            var clientTasks = await cdClient.MissionTasksTable.Where(
                t => t.Id == MissionId
            ).ToArrayAsync();

            var tasks = new List<MissionTaskBase>();

            foreach (var clientTask in clientTasks)
            {
                var taskType = (MissionTaskType) (clientTask.TaskType ?? 0);

                if (!TaskTypes.TryGetValue(taskType, out var type))
                {
                    Logger.Error($"No {nameof(MissionTaskBase)} for {taskType} found.");
                    
                    continue;
                }

                var instance = (MissionTaskBase) Activator.CreateInstance(type);

                if (instance == default || clientTask.Uid == default)
                {
                    Logger.Error($"Invalid task: {type} [{clientTask.Uid}]");
                    
                    continue;
                }
                
                await instance.LoadAsync(Player, MissionId, clientTask.Uid.Value);

                tasks.Add(instance);
            }

            Tasks = tasks.ToArray();
        }

        public async Task StartAsync()
        {
            if (HasStarted)
                return;
            
            await using var ctx = new UchuContext();
            await using var cdClient = new CdClientContext();
            
            var tasks = await cdClient.MissionTasksTable.Where(
                t => t.Id == MissionId
            ).ToArrayAsync();

            var mission = new Mission
            {
                CharacterId = Player.Id,
                MissionId = MissionId,
                Tasks = tasks.Select(task => new MissionTask
                {
                    TaskId = task.Uid ?? 0
                }).ToList()
            };

            await ctx.Missions.AddAsync(mission);
            await ctx.SaveChangesAsync();

            await UpdateMissionStateAsync(MissionState.Active);

            var clientMission = await cdClient.MissionsTable.FirstAsync(
                m => m.Id == MissionId
            );
            
            MessageMissionTypeState(MissionLockState.New, DefinedSubType, DefinedType);

            await CatchupAsync();

            if (Player.TryGetComponent<MissionInventoryComponent>(out var missionInventory))
            {
                await missionInventory.OnAcceptMission.InvokeAsync(this);
            }

            LoadMissionInstance(mission);
        }

        private async Task CatchupAsync()
        {
            var obtainTasks = Tasks.OfType<ObtainItemTask>().ToList();

            if (obtainTasks.Count != default)
            {
                if (!Player.TryGetComponent<InventoryManagerComponent>(out var inventoryManagerComponent)) return;

                var items = inventoryManagerComponent.Items;

                foreach (var task in obtainTasks)
                {
                    var toGive = items.Where(i => task.Targets.Contains((int) i.Lot)).Sum(i => i.Count);

                    for (var i = 0; i < toGive; i++)
                    {
                        await task.Progress(task.Target);

                        if (await task.IsCompleteAsync()) break;
                    }
                }
            }
        }

        public async Task CompleteAsync(int rewardItem = default)
        {
            await using var ctx = new UchuContext();
            await using var cdClient = new CdClientContext();

            //
            // If this mission is not already accepted, accept it and move on to complete it.
            //
            
            var mission = await ctx.Missions.FirstOrDefaultAsync(
                m => m.CharacterId == Player.Id && m.MissionId == MissionId
            );

            if (!HasStarted)
            {
                await StartAsync();
            }

            //
            // Save changes to be able to update its state.
            //

            await ctx.SaveChangesAsync();

            await UpdateMissionStateAsync(MissionState.Unavailable, true);

            //
            // Get character mission to complete.
            //
            
            if (mission.State == (int) MissionState.Completed) return;
            
            //
            // Update player based on rewards.
            //
            
            mission.LastCompletion = DateTimeOffset.Now.ToUnixTimeSeconds();
            mission.State = (int) MissionState.Completed;
            
            await ctx.SaveChangesAsync();

            await SendRewardsAsync(rewardItem);
            
            //
            // Set complete state
            //
            
            mission.CompletionCount++;
            
            await ctx.SaveChangesAsync();

            //
            // Inform the client it's now complete.
            //

            await UpdateMissionStateAsync(MissionState.Completed);

            if (!Player.TryGetComponent<MissionInventoryComponent>(out var missionInventory)) return;
            
            var _ = Task.Run(async () =>
            {
                await missionInventory.MissionCompleteAsync(MissionId);
            });
            
            await missionInventory.OnCompleteMission.InvokeAsync(this);
        }

        public async Task SendRewardsAsync(int rewardItem)
        {
            await using var ctx = new UchuContext();
            await using var cdClient = new CdClientContext();

            var mission = await ctx.Missions.FirstOrDefaultAsync(
                m => m.CharacterId == Player.Id && m.MissionId == MissionId
            );

            var repeat = mission.CompletionCount != 0;

            var clientMission = await cdClient.MissionsTable.FirstAsync(
                m => m.Id == MissionId
            );

            var currency = !repeat ? clientMission.Rewardcurrency ?? 0 : clientMission.Rewardcurrencyrepeatable ?? 0;
            var score = !repeat ? clientMission.LegoScore ?? 0 : 0;
            
            if (clientMission.IsMission ?? true)
            {
                // Mission

                Player.Currency += currency;

                Player.UniverseScore += score;
            }
            else
            {
                var character = await ctx.Characters.FirstAsync(
                    c => c.Id == Player.Id
                );
                
                //
                // Achievement
                //
                // These rewards have the be silent, as the client adds them itself.
                //

                character.Currency += currency;
                character.UniverseScore += score;

                //
                // The client adds currency rewards as an offset, in my testing. Therefore we
                // have to account for this offset.
                //

                Player.HiddenCurrency += currency;

                await ctx.SaveChangesAsync();
            }

            var stats = Player.GetComponent<DestroyableComponent>();

            await stats.BoostBaseHealth((uint) (clientMission.Rewardmaxhealth ?? 0));
            await stats.BoostBaseImagination((uint) (clientMission.Rewardmaximagination ?? 0));

            //
            // Get item rewards.
            //

            var inventory = Player.GetComponent<InventoryManagerComponent>();

            inventory[InventoryType.Items].Size += !repeat ? clientMission.Rewardmaxinventory ?? 0 : 0;
            
            var rewards = new (Lot, int)[]
            {
                ((repeat ? clientMission.Rewarditem1repeatable : clientMission.Rewarditem1) ?? 0,
                    (repeat ? clientMission.Rewarditem1repeatcount : clientMission.Rewarditem1count) ?? 1),

                ((repeat ? clientMission.Rewarditem2repeatable : clientMission.Rewarditem2) ?? 0,
                    (repeat ? clientMission.Rewarditem2repeatcount : clientMission.Rewarditem2count) ?? 1),

                ((repeat ? clientMission.Rewarditem3repeatable : clientMission.Rewarditem3) ?? 0,
                    (repeat ? clientMission.Rewarditem3repeatcount : clientMission.Rewarditem3count) ?? 1),

                ((repeat ? clientMission.Rewarditem4repeatable : clientMission.Rewarditem4) ?? 0,
                    (repeat ? clientMission.Rewarditem4repeatcount : clientMission.Rewarditem4count) ?? 1),
            };

            var emotes = new[]
            {
                clientMission.Rewardemote ?? -1,
                clientMission.Rewardemote2 ?? -1,
                clientMission.Rewardemote3 ?? -1,
                clientMission.Rewardemote4 ?? -1
            };

            foreach (var i in emotes.Where(e => e != -1))
            {
                await Player.UnlockEmoteAsync(i);
            }

            var isMission = clientMission.IsMission ?? true;

            var isChoice = clientMission.IsChoiceReward ?? false;

            if (isChoice)
            {
                var (lot, count) = rewards.FirstOrDefault(l => l.Item1 == rewardItem);

                count = Math.Max(count, 1);
                
                Logger.Debug($"Choice: {lot}x{count} -> {rewardItem}");
                
                var _ = Task.Run(async () => { await inventory.AddItemAsync(rewardItem, (uint) count); });
            }
            else
            {
                foreach (var (rewardLot, rewardCount) in rewards)
                {
                    var lot = rewardLot;
                    var count = Math.Max(rewardCount, 1);

                    if (lot <= 0) continue;

                    if (isMission)
                    {
                        var _ = Task.Run(async () => { await inventory.AddItemAsync(lot, (uint) count); });
                    }
                    else
                    {
                        var _ = Task.Run(async () =>
                        {
                            await Task.Delay(10000);

                            await inventory.AddItemAsync(lot, (uint) count);
                        });
                    }
                }
            }
        }

        public async Task<bool> IsCompleteAsync()
        {
            foreach (var task in Tasks)
            {
                var isComplete = await task.IsCompleteAsync();

                if (!isComplete) return false;
            }

            return true;
        }

        public async Task SoftCompleteAsync()
        {
            await using var cdClient = new CdClientContext();

            var clientMission = await cdClient.MissionsTable.FirstAsync(
                m => m.Id == MissionId
            );

            if (clientMission.IsMission ?? true)
            {
                await UpdateMissionStateAsync(MissionState.ReadyToComplete);
                
                return;
            }

            await CompleteAsync();
        }

        public async Task UpdateMissionStateAsync(MissionState state, bool sendingRewards = false)
        {
            await using (var ctx = new UchuContext())
            {
                var mission = await ctx.Missions.FirstOrDefaultAsync(
                    m => m.CharacterId == Player.Id && m.MissionId == MissionId
                );

                mission.State = (int) state;
                await ctx.SaveChangesAsync();
            }

            State = state;
            
            Player.Message(new NotifyMissionMessage
            {
                Associate = Player,
                MissionId = MissionId,
                MissionState = state,
                SendingRewards = sendingRewards
            });
        }

        public void MessageMissionTypeState(MissionLockState state, string subType, string type)
        {
            Player.Message(new SetMissionTypeStateMessage
            {
                Associate = Player,
                LockState = state,
                SubType = subType,
                Type = type
            });
        }
    }
}