using System.Threading.Tasks;
using Uchu.Core;

namespace Uchu.World.Systems.Missions
{
    public class DiscoverTask : MissionTaskInstance
    {
        public DiscoverTask(MissionInstance mission, int taskId, int missionTaskIndex)
            : base(mission, taskId, missionTaskIndex)
        {
        }

        public override MissionTaskType Type => MissionTaskType.Discover;

        public override bool Completed => Progress.Contains(1);

        public async Task ReportProgress(string poiGroup)
        {
            Logger.Information($"Report progress {poiGroup}");
            if (TargetString != poiGroup)
                return;

            AddProgress(1);

            if (Completed)
                await CheckMissionCompletedAsync();
        }
    }
}