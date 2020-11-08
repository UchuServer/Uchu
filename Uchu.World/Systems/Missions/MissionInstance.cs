using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Uchu.Core;
using Uchu.Core.Client;

namespace Uchu.World.Systems.Missions
{
    /// <summary>
    /// Combined instance of the CdClient mission template and the Uchu data base mission progress.
    /// </summary>
    /// <remarks>
    /// Can be loaded in on world load or started during gameplay to be used for the player throughout the world.
    /// </remarks>
    public class MissionInstance
    {
        #region properties
        // Template properties
        /// <summary>
        /// The player that started this mission
        /// </summary>
        public Player Player { get; }
        
        /// <summary>
        /// The id of the mission in the CdClient
        /// </summary>
        public int MissionId { get; }
        
        /// <summary>
        /// Whether this is a mission or an achievement
        /// </summary>
        public bool IsMission { get; private set; }
        
        /// <summary>
        /// If this mission has a reward that the player has to explicitly choose (e.g. one of 3 weapons).
        /// </summary>
        public bool IsChoiceReward { get; private set; }
        
        /// <summary>
        /// Meta information regarding the type of this mission
        /// </summary>
        public string DefinedType { get; private set; }
        
        /// <summary>
        /// Meta information regarding the sub type of this mission
        /// </summary>
        public string DefinedSubType { get; private set; }
        
        /// <summary>
        /// All the tasks that need to be completed for this mission
        /// </summary>
        public HashSet<MissionTaskInstance> Tasks { get; private set; }
        
        /// <summary>
        /// Currency that's rewarded once the mission is completed
        /// </summary>
        public long RewardCurrency { get; private set; }
        
        /// <summary>
        /// Currency that's rewarded once the mission has been completed more than once
        /// </summary>
        public long RewardCurrencyRepeatable { get; private set; }
        
        /// <summary>
        /// Lego Score that's rewarded once the mission has been completed
        /// </summary>
        public int RewardScore { get; private set; }
        
        /// <summary>
        /// Maximum health boost that's rewarded once the mission has been completed
        /// </summary>
        public int RewardMaxHealth { get; private set; }
        
        /// <summary>
        /// Maximum imagination boost that's rewarded once the mission has been completed
        /// </summary>
        public int RewardMaxImagination { get; private set; }
        
        /// <summary>
        /// Maximum inventory boost that's rewarded once the mission has been completed
        /// </summary>
        public int RewardMaxInventory { get; private set; }
        
        /// <summary>
        /// Optional emote that's rewarded once the mission is completed
        /// </summary>
        public int RewardEmote1 { get; private set; }
        
        /// <summary>
        /// Optional emote that's rewarded once the mission is completed
        /// </summary>
        public int RewardEmote2 { get; private set; }
        
        /// <summary>
        /// Optional emote that's rewarded once the mission is completed
        /// </summary>
        public int RewardEmote3 { get; private set; }
        
        /// <summary>
        /// Optional emote that's rewarded once the mission is completed
        /// </summary>
        public int RewardEmote4 { get; private set; }
        
        /// <summary>
        /// Optional item that's rewarded once the mission is completed
        /// </summary>
        public int RewardItem1 { get; private set; }
        
        /// <summary>
        /// The amount of the first item that's rewarded once the mission is completed
        /// </summary>
        public int RewardItem1Count { get; private set; }
        
        /// <summary>
        /// Optional item that's rewarded once the mission is completed more than once
        /// </summary>
        public int RewardItem1Repeatable { get; private set; }
        
        /// <summary>
        /// The amount of the first item that's rewarded once the mission is completed more than once
        /// </summary>
        public int RewardItem1RepeatableCount { get; private set; }
        
        /// <summary>
        /// Optional item that's rewarded once the mission is completed
        /// </summary>
        public int RewardItem2 { get; private set; }
        
        /// <summary>
        /// The amount of the second item that's rewarded once the mission is completed
        /// </summary>
        public int RewardItem2Count { get; private set; }
        
        /// <summary>
        /// Optional item that's rewarded once the mission is completed more than once
        /// </summary>
        public int RewardItem2Repeatable { get; private set; }
        
        /// <summary>
        /// The amount of the second item that's rewarded once the mission is completed more than once
        /// </summary>
        public int RewardItem2RepeatableCount { get; private set; }
        
        /// <summary>
        /// Optional item that's rewarded once the mission is completed
        /// </summary>
        public int RewardItem3 { get; private set; }
        
        /// <summary>
        /// The amount of the third item that's rewarded once the mission is completed
        /// </summary>
        public int RewardItem3Count { get; private set; }
        
        /// <summary>
        /// Optional item that's rewarded once the mission is completed more than once
        /// </summary>
        public int RewardItem3Repeatable { get; private set; }
        
        /// <summary>
        /// The amount of the third item that's rewarded once the mission is completed more than once
        /// </summary>
        public int RewardItem3RepeatableCount { get; private set; }
        
        /// <summary>
        /// Optional item that's rewarded once the mission is completed
        /// </summary>
        public int RewardItem4 { get; private set; }
        
        /// <summary>
        /// The amount of the fourth item that's rewarded once the mission is completed
        /// </summary>
        public int RewardItem4Count { get; private set; }
        
        /// <summary>
        /// Optional item that's rewarded once the mission is completed more than once
        /// </summary>
        public int RewardItem4Repeatable { get; private set; }
        
        /// <summary>
        /// The amount of fourth item that's rewarded once the mission is completed more than once
        /// </summary>
        public int RewardItem4RepeatableCount { get; private set; }
        
        // Instance properties
        /// <summary>
        /// The current state of this mission for the player
        /// </summary>
        public MissionState State { get; private set; }
        
        /// <summary>
        /// The amount of times the player completed this mission
        /// </summary>
        public int CompletionCount { get; private set; }
        
        /// <summary>
        /// The last time the player completed this mission
        /// </summary>
        public long LastCompletion { get; private set; }
        
        /// <summary>
        /// If this player is repeating this mission
        /// </summary>
        public bool Repeat => CompletionCount != 0;
        
        /// <summary>
        /// Map that contains all the possible mission tasks with their associated assembly types
        /// </summary>
        private static Dictionary<MissionTaskType, Type> TaskTypes { get; }
        #endregion properties
        
        #region fields
        /// <summary>
        /// If this mission has loaded CdClient information
        /// </summary>
        private bool _loaded = false;
        
        /// <summary>
        /// If this mission has loaded Uchu database information
        /// </summary>
        private bool _instantiated = false;
        #endregion fields

        static MissionInstance()
        {
            TaskTypes = new Dictionary<MissionTaskType, Type>();
            
            // Get all possible mission task types
            var types = typeof(MissionInstance).Assembly.GetTypes().Where(
                t => t.BaseType == typeof(MissionTaskInstance)
            );
            
            foreach (var type in types)
            {
                if (type.IsAbstract)
                    continue;
                
                var instance = (MissionTaskInstance) Activator.CreateInstance(type);

                try
                {
                    if (instance != null)
                        TaskTypes[instance.Type] = type;
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
        public async Task LoadMissionTemplateAsync()
        {
            await using var cdClient = new CdClientContext();
            
            var mission = await cdClient.MissionsTable.FirstAsync(
                m => m.Id == MissionId
            );
            
            IsMission = mission.IsMission ?? true;
            IsChoiceReward = mission.IsChoiceReward ?? false;
            DefinedType = mission.Definedtype;
            DefinedSubType = mission.Definedsubtype;
            
            // Possible stat rewards
            RewardMaxHealth = mission.Rewardmaxhealth ?? 0;
            RewardMaxImagination = mission.Rewardmaximagination ?? 0;
            RewardMaxInventory = mission.Rewardmaxinventory ?? 0;
            RewardCurrency = mission.Rewardcurrency ?? 0;
            RewardCurrencyRepeatable = mission.Rewardcurrencyrepeatable ?? 0;
            RewardScore = mission.LegoScore ?? 0;
            
            // Emotes
            RewardEmote1 = mission.Rewardemote ?? -1;
            RewardEmote2 = mission.Rewardemote2 ?? -1;
            RewardEmote3 = mission.Rewardemote3 ?? -1;
            RewardEmote4 = mission.Rewardemote4 ?? -1;
            
            // First optional reward item
            RewardItem1 = mission.Rewarditem1 ?? 0;
            RewardItem1Count = mission.Rewarditem1count ?? 1;
            RewardItem1Repeatable = mission.Rewarditem1repeatable ?? 0;
            RewardItem1RepeatableCount = mission.Rewarditem1repeatcount ?? 1;
            
            // Second optional reward item
            RewardItem2 = mission.Rewarditem2 ?? 0;
            RewardItem2Count = mission.Rewarditem2count ?? 1;
            RewardItem2Repeatable = mission.Rewarditem2repeatable ?? 0;
            RewardItem2RepeatableCount = mission.Rewarditem2repeatcount ?? 1;
            
            // Third optional reward item
            RewardItem3 = mission.Rewarditem3 ?? 0;
            RewardItem3Count = mission.Rewarditem3count ?? 1;
            RewardItem3Repeatable = mission.Rewarditem3repeatable ?? 0;
            RewardItem3RepeatableCount = mission.Rewarditem3repeatcount ?? 1;
            
            // Fourth optional reward item
            RewardItem4 = mission.Rewarditem4 ?? 0;
            RewardItem4Count = mission.Rewarditem4count ?? 1;
            RewardItem4Repeatable = mission.Rewarditem4repeatable ?? 0;
            RewardItem4RepeatableCount = mission.Rewarditem4repeatcount ?? 1;

            var clientTasks = await cdClient.MissionTasksTable.Where(
                t => t.Id == MissionId
            ).ToArrayAsync();

            // Load all the tasks for this mission
            var tasks = new HashSet<MissionTaskInstance>();
            foreach (var clientTask in clientTasks)
            {
                var taskType = (MissionTaskType) (clientTask.TaskType ?? 0);
                if (!TaskTypes.TryGetValue(taskType, out var type))
                {
                    Logger.Error($"No {nameof(MissionTaskInstance)} for {taskType} found.");
                    continue;
                }

                var instance = (MissionTaskInstance) Activator.CreateInstance(type);
                if (instance == default || clientTask.Uid == default)
                {
                    Logger.Error($"Invalid task: {type} [{clientTask.Uid}]");
                    continue;
                }
                
                await instance.LoadAsync(Player, MissionId, clientTask.Uid.Value);
                tasks.Add(instance);
            }

            Tasks = tasks;
            _loaded = true;
        }
        
        /// <summary>
        /// Saves the current in-memory mission instance to the database
        /// </summary>
        private async Task SaveMissionInstanceAsync()
        {
            if (!_instantiated)
                throw new InvalidOperationException($"Can't save a mission instance that's not instantiated. " +
                                                    $"Call {nameof(LoadMissionInstance)} first.");
            
            await using var ctx = new UchuContext();
            
            var mission = await ctx.Missions.FirstOrDefaultAsync(
                m => m.CharacterId == Player.Id && m.MissionId == MissionId
            );

            mission.State = (int) State;
            mission.CompletionCount = CompletionCount;
            mission.LastCompletion = LastCompletion;

            await ctx.SaveChangesAsync();
        }
        
        /// <summary>
        /// Loads specific UchuContext information about the mission.
        /// </summary>
        private void LoadMissionInstance(Mission mission)
        {
            State = (MissionState)mission.State;
            CompletionCount = mission.CompletionCount;
            LastCompletion = mission.LastCompletion;
            _instantiated = true;
        }

        /// <summary>
        /// Loads specific uchu context information about the mission
        /// </summary>
        private async Task LoadMissionInstanceAsync()
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

        /// <summary>
        /// Loads the mission template from the cd client and optionally the mission instance from the uchu database
        /// if it's been started before.
        /// </summary>
        public async Task LoadAsync()
        {
            await LoadMissionTemplateAsync();
            await LoadMissionInstanceAsync();
        }

        /// <summary>
        /// Creates a new mission instance in the database from the template and stores it in memory
        /// </summary>
        private async Task CreateInstanceAsync()
        {
            await using var ctx = new UchuContext();
            
            var mission = new Mission
            {
                CharacterId = Player.Id,
                MissionId = MissionId,
                State = (int)MissionState.Active,
                Tasks = Tasks.Select(task => new MissionTask
                {
                    TaskId = task.TaskId
                }).ToList()
            };

            await ctx.Missions.AddAsync(mission);
            await ctx.SaveChangesAsync();
            
            // Now that the mission is created, save everything in memory and notify the client
            LoadMissionInstance(mission);
        }

        /// <summary>
        /// Starts this mission instance and notifies the client
        /// </summary>
        public async Task StartAsync()
        {
            if (_instantiated)
                return;

            await CreateInstanceAsync();
            
            MessageNotifyMission();
            MessageMissionTypeState(MissionLockState.New);
            
            await SyncObtainItemTasksWithInventory();

            if (Player.TryGetComponent<MissionInventoryComponent>(out var missionInventory))
            {
                await missionInventory.OnAcceptMission.InvokeAsync(this);
            }
        }

        /// <summary>
        /// Syncs the obtain item tasks of a player with the inventory they currently have, can be used to shorttrack
        /// newly started tasks.
        /// </summary>
        private async Task SyncObtainItemTasksWithInventory()
        {
            var obtainTasks = Tasks.OfType<ObtainItemTask>().ToList();

            if (obtainTasks.Count > 0)
            {
                if (!Player.TryGetComponent<InventoryManagerComponent>(out var inventoryManagerComponent))
                    return;

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

        /// <summary>
        /// Completes this mission by updating the state, notifying the player and handing out all the rewards
        /// </summary>
        /// <param name="rewardItem">If this mission had a reward choice, this item will be chosen as reward</param>
        /// <exception cref="InvalidOperationException">If this mission hasn't been loaded yet</exception>
        public async Task CompleteAsync(int rewardItem = default)
        {
            if (!_loaded)
                throw new InvalidOperationException("Can't complete mission as it's not been loaded yet. " +
                                                    $"Call {nameof(LoadAsync)} before calling {nameof(CompleteAsync)}.");
            if (!_instantiated)
                await StartAsync();

            if (State == MissionState.Completed)
                return;
            
            await UpdateMissionStateAsync(MissionState.Unavailable, true);
            await SendRewardsAsync(rewardItem);
            
            LastCompletion = DateTimeOffset.Now.ToUnixTimeSeconds();
            State = MissionState.Completed;
            CompletionCount++;
            
            await SaveMissionInstanceAsync();
            MessageNotifyMission();

            if (!Player.TryGetComponent<MissionInventoryComponent>(out var missionInventory))
                return;
            
            var _ = Task.Run(async () =>
            {
                await missionInventory.MissionCompleteAsync(MissionId);
            });
            
            await missionInventory.OnCompleteMission.InvokeAsync(this);
        }

        /// <summary>
        /// Sends the client all the rewards associated with this mission
        /// </summary>
        /// <param name="rewardItem">A specific item that should be rewarded, only
        /// rewarded if it's in one of the mission rewards.</param>
        private async Task SendRewardsAsync(int rewardItem)
        {
            await RewardPlayerCurrency();
            await RewardPlayerEmotes();
            await RewardPlayerStats();
            RewardPlayerLoot(rewardItem);
        }


        /// <summary>
        /// Rewards the player with currency based on the mission template
        /// </summary>
        /// <remarks>
        /// If this is an achievement the currency is updated silently without a notify message
        /// as the client updates the currency locally.
        /// </remarks>
        private async Task RewardPlayerCurrency()
        {
            var currency = !Repeat ? RewardCurrencyRepeatable : RewardCurrency;
            var score = Repeat ? 0 : RewardScore;
            
            if (IsMission)
            {
                Player.Currency += currency;
                Player.UniverseScore += score;
            }
            else
            {
                await using var ctx = new UchuContext();
                
                var character = await ctx.Characters.FirstAsync(
                    c => c.Id == Player.Id
                );
                
                // Achievement, client adds these itself so we don't need to notify
                character.Currency += currency;
                character.UniverseScore += score;

                // The client adds currency rewards as an offset, in my testing. Therefore we
                // have to account for this offset.
                Player.HiddenCurrency += currency;

                await ctx.SaveChangesAsync();
            }
        }
        
        /// <summary>
        /// Rewards the player with emotes that might be unlocked from completing this mission
        /// </summary>
        private async Task RewardPlayerEmotes()
        {
            var emotes = new[] { RewardEmote1, RewardEmote2, RewardEmote3, RewardEmote4 };
            foreach (var i in emotes.Where(e => e != -1))
            {
                await Player.UnlockEmoteAsync(i);
            }
        }

        /// <summary>
        /// Rewards the mission instance player with the stat rewards associated to this mission
        /// </summary>
        private async Task RewardPlayerStats()
        {
            var stats = Player.GetComponent<DestroyableComponent>();
            await stats.BoostBaseHealth((uint) RewardMaxHealth);
            await stats.BoostBaseImagination((uint) RewardMaxImagination);
        }

        /// <summary>
        /// Rewards the player by giving them the optional loot associated with this mission and also
        /// optionally updating their bag size.
        /// </summary>
        /// <param name="rewardItem">A specific item that should be rewarded, only
        /// rewarded if it's in one of the mission rewards.</param>
        private void RewardPlayerLoot(int rewardItem)
        {
            var inventory = Player.GetComponent<InventoryManagerComponent>();
            inventory[InventoryType.Items].Size += Repeat ? 0 : RewardMaxInventory;
            
            var rewards = new (Lot, int)[]
            {
                (Repeat ? RewardItem1Repeatable : RewardItem1, Repeat ? RewardItem1RepeatableCount : RewardItem1Count),
                (Repeat ? RewardItem2Repeatable : RewardItem2, Repeat ? RewardItem2RepeatableCount : RewardItem2Count),
                (Repeat ? RewardItem3Repeatable : RewardItem3, Repeat ? RewardItem3RepeatableCount : RewardItem3Count),
                (Repeat ? RewardItem4Repeatable : RewardItem4, Repeat ? RewardItem4RepeatableCount : RewardItem4Count),
            };

            if (IsChoiceReward)
            {
                var (_, count) = rewards.FirstOrDefault(l => l.Item1 == rewardItem);
                count = Math.Max(count, 1);
                
                var _ = Task.Run(async () =>
                {
                    await inventory.AddItemAsync(rewardItem, (uint) count);
                });
            }
            else
            {
                foreach (var (rewardLot, rewardCount) in rewards)
                {
                    var lot = rewardLot;
                    var count = Math.Max(rewardCount, 1);

                    if (lot <= 0)
                        continue;

                    if (IsMission)
                    {
                        var _ = Task.Run(async () =>
                        {
                            await inventory.AddItemAsync(lot, (uint) count);
                        });
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

        /// <summary>
        /// Checks if this mission is completed by checking if all sub-tasks are completed
        /// </summary>
        /// <returns><c>true</c> if completed, <c>false</c> otherwise</returns>
        public async Task<bool> IsCompleteAsync()
        {
            foreach (var task in Tasks)
            {
                var isComplete = await task.IsCompleteAsync();
                if (!isComplete)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// If this mission is an achievement, completes it. If this mission is a mission, allows the player to complete
        /// it at the mission giver.
        /// </summary>
        public async Task SoftCompleteAsync()
        {
            if (IsMission)
            {
                await UpdateMissionStateAsync(MissionState.ReadyToComplete);
                return;
            }

            await CompleteAsync();
        }

        /// <summary>
        /// Updates the mission state by saving to the database and notifying the client
        /// </summary>
        /// <remarks>
        /// When editing multiple properties of the mission instance, set <c>State</c> explicitly,
        /// save using <c>SaveInstanceAsync</c> and notify the client using <c>MessageNotifyMission</c>
        /// afterwards, this results in less database sessions.
        /// </remarks>
        /// <param name="state">The new state to set this mission to</param>
        /// <param name="sendingRewards">Whether or not this state will result in rewards</param>
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
            MessageNotifyMission(sendingRewards);
        }

        /// <summary>
        /// Notifies a player of the state of an active mission
        /// </summary>
        /// <param name="sendingRewards"></param>
        private void MessageNotifyMission(bool sendingRewards = false)
        {
            Player.Message(new NotifyMissionMessage
            {
                Associate = Player,
                MissionId = MissionId,
                MissionState = State,
                SendingRewards = sendingRewards
            });
        }

        private void MessageMissionTypeState(MissionLockState state)
        {
            Player.Message(new SetMissionTypeStateMessage
            {
                Associate = Player,
                LockState = state,
                SubType = DefinedSubType,
                Type = DefinedType
            });
        }
    }
}