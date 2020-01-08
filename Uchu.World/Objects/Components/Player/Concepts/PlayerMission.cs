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
    public class PlayerMission : Object
    {
        public int MissionId { get; set; }

        public Player Player { get; set; }
        
        public bool Achievement { get; private set; }

        internal PlayerMission()
        {
            Listen(OnStart, () =>
            {
                using var cdClient = new CdClientContext();
            
                var mission = cdClient.MissionsTable.FirstOrDefault(m => m.Id == MissionId);

                if (mission == default)
                {
                    Logger.Error($"{MissionId} is not a valid mission.");
                    
                    return;
                }

                Achievement = !(mission.IsMission ?? false);
            });
        }

        public async Task<MissionState> GetStateAsync()
        {
            return (MissionState) (await GetMissionAsync()).State;
        }
        
        public async Task UpdateStateAsync(MissionState state, bool sendingRewards)
        {
            await SetMissionState(state);
            
            Player.Message(new NotifyMissionMessage
            {
                Associate = Player,
                MissionId = MissionId,
                MissionState = state,
                SendingRewards = sendingRewards
            });
        }

        public void UpdateInfo(MissionLockState state, string subType, string type)
        {
            Player.Message(new SetMissionTypeStateMessage
            {
                Associate = Player,
                LockState = state,
                SubType = subType,
                Type = type
            });
        }
        
        public void UpdateTask(int taskIndex, float[] updates)
        {
            Player.Message(new NotifyMissionTaskMessage
            {
                Associate = Player,
                MissionId = MissionId,
                TaskIndex = taskIndex,
                Updates = updates
            });
        }

        public async Task CompleteAsync(Lot rewardItem)
        {
            await using var ctx = new UchuContext();
            await using var cdClient = new CdClientContext();

            //
            // Get mission information.
            //

            var mission = cdClient.MissionsTable.FirstOrDefault(m => m.Id == MissionId);

            if (mission == default) return;

            //
            // Get character information.
            //

            var character = ctx.Characters
                .Include(c => c.Items)
                .Include(c => c.Missions)
                .ThenInclude(m => m.Tasks)
                .Single(c => c.CharacterId == Player.ObjectId);

            //
            // If this mission is not already accepted, accept it and move on to complete it.
            //

            if (!character.Missions.Exists(m => m.MissionId == MissionId))
            {
                var tasks = cdClient.MissionTasksTable.Where(t => t.Id == MissionId);

                var characterTasks = new List<MissionTask>();

                foreach (var task in tasks)
                {
                    characterTasks.Add(await GetTask(task));
                }
                
                character.Missions.Add(new Mission
                {
                    MissionId = MissionId,
                    State = (int) MissionState.Active,
                    Tasks = characterTasks
                });
            }

            //
            // Save changes to be able to update its state.
            //

            ctx.SaveChanges();

            await UpdateStateAsync(MissionState.Unavailable, true);

            //
            // Get character mission to complete.
            //

            var characterMission = character.Missions.Find(m => m.MissionId == MissionId);

            if (characterMission.State == (int) MissionState.Completed) return;

            var repeat = characterMission.CompletionCount != 0;
            characterMission.CompletionCount++;
            characterMission.LastCompletion = DateTimeOffset.Now.ToUnixTimeSeconds();

            //
            // Inform the client it's now complete.
            //

            await UpdateStateAsync(MissionState.Completed, false);

            characterMission.State = (int) MissionState.Completed;

            ctx.SaveChanges();

            //
            // Update player based on rewards.
            //

            if (!Achievement)
            {
                // Mission

                Player.Currency += mission.Rewardcurrency ?? 0;

                Player.UniverseScore += mission.LegoScore ?? 0;
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

                Player.HiddenCurrency += mission.Rewardcurrency ?? 0;

                ctx.SaveChanges();
            }

            var stats = Player.GetComponent<Stats>();

            stats.MaxHealth += (uint) (mission.Rewardmaxhealth ?? 0);
            stats.MaxImagination += (uint) (mission.Rewardmaximagination ?? 0);

            //
            // Get item rewards.
            //

            var inventory = Player.GetComponent<InventoryManagerComponent>();

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
                    if (lot == default || count == default) continue;

                    await inventory.AddItemAsync(lot, (uint) count);
                }
            }
            else
            {
                var (lot, count) = rewards.FirstOrDefault(l => l.Item1 == rewardItem);

                if (lot != default && count != default) await inventory.AddItemAsync(lot, (uint) count);
            }
        }

        internal async Task SetMissionState(MissionState state)
        {
            await using var ctx = new UchuContext();

            var mission = await ctx.Missions.FirstOrDefaultAsync(
                m => m.CharacterId == Player.ObjectId && m.MissionId == MissionId
            );

            mission.State = (int) state;
        }

        internal async Task<Mission> GetMissionAsync()
        {
            await using var ctx = new UchuContext();

            var mission = await ctx.Missions.FirstOrDefaultAsync(
                m => m.CharacterId == Player.ObjectId && m.MissionId == MissionId
            );
            
            return mission;
        }
        
        internal async Task<MissionTask> GetTask(MissionTasks task)
        {
            await using var ctx = new UchuContext();
            
            var character = ctx.Characters
                .Include(c => c.Items)
                .Include(c => c.Missions)
                .ThenInclude(m => m.Tasks)
                .Single(c => c.CharacterId == Player.ObjectId);
            
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