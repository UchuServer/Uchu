using System.Linq;
using System.Threading.Tasks;

namespace Uchu.World.Systems.Missions
{
    public class CollectTask : MissionTaskInstance
    {
        public CollectTask(MissionInstance mission, int taskId, int missionTaskIndex) 
            : base(mission, taskId, missionTaskIndex)
        {
        }

        public CollectTask(MissionInstance mission, MissionTaskInstance cachedInstance) : base(mission, cachedInstance)
        {
        }
        
        public override MissionTaskType Type => MissionTaskType.Collect;
        
        public async Task ReportProgress(GameObject gameObject)
        {
            var component = gameObject.GetComponent<CollectibleComponent>();

            if (!Targets.Contains((int) gameObject.Lot))
                return;

            var shiftedId = component.CollectibleId + ((int) gameObject.Zone.ZoneId << 8);

            AddProgress(shiftedId);

            if (Completed)
                await CheckMissionCompletedAsync();
        }
    }
}