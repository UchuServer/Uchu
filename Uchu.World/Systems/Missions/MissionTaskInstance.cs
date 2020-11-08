using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Uchu.Core;
using Uchu.Core.Client;

namespace Uchu.World.Systems.Missions
{
    public abstract class MissionTaskInstance
    {
        public MissionTaskInstance(MissionInstance mission, int taskId)
        {
            Mission = mission;
            TaskId = taskId;
        }
        
        #region properties
        public MissionInstance Mission { get; private set; }
        
        public int TaskId { get; private set; }
        
        public int Target { get; private set; }
        
        public int[] TargetGroup { get; private set; }

        public int[] Targets
        {
            get
            {
                var targets = TargetGroup?.ToList() ?? new List<int>();

                targets.Add(Target);

                return targets.ToArray();
            }
        }
        
        public int TargetValue { get; private set; }
        
        public int[] Parameters { get; private set; }
        
        public abstract MissionTaskType Type { get; }
        #endregion properties

        public async Task LoadAsync()
        {
            await using var cdClient = new CdClientContext();
            await LoadAsync(cdClient);
        }
        
        public async Task LoadAsync(CdClientContext cdClient)
        {
            var clientTask = await cdClient.MissionTasksTable.FirstAsync(
                t => t.Uid == TaskId && t.Id == Mission.MissionId
            );

            Target = clientTask.Target ?? 0;
            TargetValue = clientTask.TargetValue ?? 0;

            if (clientTask.TargetGroup != default)
            {
                var targetGroup = clientTask.TargetGroup
                    .Replace(" ", "")
                    .Split(',')
                    .Where(c => !string.IsNullOrEmpty(c)).ToList();
                
                var targets = new List<int>();

                foreach (var target in targetGroup)
                {
                    if (int.TryParse(target, out var lot))
                    {
                        targets.Add(lot);
                    }
                }

                TargetGroup = targets.ToArray();
            }
            else
            {
                TargetGroup = new int[0];
            }

            if (clientTask.TaskParam1 != default)
            {
                var targetParameters = clientTask.TaskParam1
                    .Replace(" ", "")
                    .Split(',')
                    .Where(c => !string.IsNullOrEmpty(c)).ToList();
                
                var parameters = new List<int>();

                foreach (var parameter in targetParameters)
                {
                    if (int.TryParse(parameter, out var lot))
                    {
                        parameters.Add(lot);
                    }
                }

                Parameters = parameters.ToArray();
            }
            else
            {
                Parameters = new int[0];
            }
        }
        
        public async Task AddProgressAsync(float value)
        {
            await using var ctx = new UchuContext();

            var mission = await ctx.Missions
                .Include(m => m.Tasks)
                .ThenInclude(t => t.Values)
                .FirstAsync(m => m.MissionId == MissionId && m.CharacterId == Player.Id);

            var task = mission.Tasks.First(m => m.TaskId == TaskId);

            task.Add(value);

            await ctx.SaveChangesAsync();

            await MessageUpdateMissionTaskAsync(new[] {(float) task.ValueArray().Length});
        }

        public async Task<int> GetProgressAsync()
        {
            await using var ctx = new UchuContext();
            
            var mission = await ctx.Missions
                .Include(m => m.Tasks)
                .ThenInclude(t => t.Values)
                .FirstAsync(m => m.MissionId == MissionId && m.CharacterId == Player.Id);
            
            var task = mission.Tasks.First(m => m.TaskId == TaskId);

            return task.ValueLength();
        }
        
        public async Task<float[]> GetProgressValuesAsync()
        {
            await using var ctx = new UchuContext();

            var mission = await ctx.Missions
                .Include(m => m.Tasks)
                .ThenInclude(t => t.Values)
                .FirstAsync(m => m.MissionId == MissionId && m.CharacterId == Player.Id);
            
            var task = mission.Tasks.First(m => m.TaskId == TaskId);

            return task.ValueArray();
        }

        public async Task MessageUpdateMissionTaskAsync(float[] updates)
        {
            await using var cdClient = new CdClientContext();

            var clientTasks = await cdClient.MissionTasksTable.Where(
                t => t.Id == MissionId
            ).ToArrayAsync();

            var clientTask = clientTasks.First(c => c.Uid == TaskId);
            
            Player.Message(new NotifyMissionTaskMessage
            {
                Associate = Player,
                MissionId = MissionId,
                TaskIndex = clientTasks.IndexOf(clientTask),
                Updates = updates
            });
        }

        public virtual async Task<bool> IsCompleteAsync()
        {
            var progress = await GetProgressAsync();

            return progress == TargetValue;
        }''

        public async Task CheckMissionCompleteAsync()
        {
            var component = Player.GetComponent<MissionInventoryComponent>();

            var mission = component.MissionInstances.First(m => m.MissionId == MissionId);

            var isComplete = await mission.IsCompleteAsync();
            
            if (!isComplete) return;

            await mission.SoftCompleteAsync();
        }
    }
}