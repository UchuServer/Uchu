using System.Linq;
using System.Threading.Tasks;

namespace Uchu.World.Systems.Missions
{
    public class UseEmoteTask : MissionTaskInstance
    {
        public UseEmoteTask(MissionInstance mission, int taskId, int missionTaskIndex)
            : base(mission, taskId, missionTaskIndex)
        {
        }

        public UseEmoteTask(MissionInstance mission, MissionTaskInstance cachedInstance) : base(mission, cachedInstance)
        {
        }
        
        public override MissionTaskType Type => MissionTaskType.UseEmote;

        public override bool Completed => Progress.Contains(Parameters.FirstOrDefault());

        public async Task ReportProgress(GameObject gameObject, int emote)
        {
            if (gameObject.Lot != Target || Parameters.FirstOrDefault() != emote)
                return;

            AddProgress(emote);

            if (Completed)
                await CheckMissionCompletedAsync();
        }
    }
}