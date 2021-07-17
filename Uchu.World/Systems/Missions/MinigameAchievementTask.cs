using System.Linq;
using System.Threading.Tasks;

namespace Uchu.World.Systems.Missions
{
    public class MinigameAchievementTask : MissionTaskInstance
    {
        public MinigameAchievementTask(MissionInstance mission, int taskId, int missionTaskIndex) 
            : base(mission, taskId, missionTaskIndex)
        {
        }

        public override MissionTaskType Type => MissionTaskType.MinigameAchievement;

        /// <summary>
        /// Called when a player completed a mission, provides the mission that was completed
        /// </summary>
        public override bool Completed => Progress.Any(progress => progress >= RequiredProgress);

        /// <summary>
        /// Progresses minigame tasks
        /// </summary>
        /// <param name="activityId">Activity id of the minigame.</param>
        /// <param name="targetGroup">Target group to progress.</param>
        /// <param name="value">Value to progress with.</param>
        public async Task ReportProgress(int activityId, string targetGroup, float value)
        {
            // Return if the activity isn't completed.
            if (activityId != this.Target) return;
            if (targetGroup != this.TargetString) return;
            if (value < this.RequiredProgress) return;

            // Add the progress.
            AddProgress(value);
            if (Completed)
                await CheckMissionCompletedAsync();
        }
    }
}