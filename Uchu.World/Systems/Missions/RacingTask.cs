using System.Linq;
using System.Threading.Tasks;
using Uchu.Core;

namespace Uchu.World.Systems.Missions
{
    public class RacingTask : MissionTaskInstance
    {
        public RacingTask(MissionInstance mission, int taskId, int missionTaskIndex)
            : base(mission, taskId, missionTaskIndex)
        {
        }

        public override MissionTaskType Type => MissionTaskType.Racing;

        public override bool Completed => IsCompleted();

        public async Task ReportRaceDone(ZoneId zoneId, uint place, long racetime, uint wrecks, int playerCount)
        {
            if (!Targets.Contains(zoneId))
                return;

            int minPlayerCount = 0; // TODO: 3;
            switch ((RacingTaskType)Parameters[0])
            {
                case RacingTaskType.Rank:
                    if (place <= RequiredProgress && playerCount >= minPlayerCount)
                        await AddProgressAndCheck(zoneId);
                    break;

                case RacingTaskType.Racetime:
                    if (racetime <= RequiredProgress)
                        await AddProgressAndCheck(zoneId);
                    break;

                case RacingTaskType.Wrecks:
                    if (wrecks <= 0)
                        await AddProgressAndCheck(zoneId);
                    break;

                case RacingTaskType.WinInWorld:
                case RacingTaskType.WinInWorlds:
                    if (place <= 1 && playerCount >= minPlayerCount)
                        await AddProgressAndCheck(zoneId);
                    break;

                case RacingTaskType.FinishLast:
                    if (place >= playerCount)
                        await AddProgressAndCheck(zoneId);
                    break;
            }
        }

        public async Task ReportLaptime(int laptime, ZoneId zoneId)
        {
            if (!IsValid(RacingTaskType.Laptime, zoneId))
                return;

            if (laptime <= RequiredProgress)
                await AddProgressAndCheck(zoneId);
        }

        public async Task ReportMissionComplete(int missionId)
        {
            var taskType = Parameters[0];
            if (taskType != (int)RacingTaskType.Missions && taskType != (int)RacingTaskType.ZoneMissions)
                return;

            if (!Targets.Contains(missionId) || Progress.Contains(missionId))
                return;

            await AddProgressAndCheck(missionId);
        }

        public async Task ReportSmash(Lot lot, ZoneId zoneId)
        {
            if (IsValid(RacingTaskType.SmashAny, zoneId))
                await AddProgressAndCheck(zoneId);
            else if (IsValid(RacingTaskType.Smash, lot))
                await AddProgressAndCheck(lot);
        }

        public async Task ReportImagination(ZoneId zoneId)
        {
            if (!IsValid(RacingTaskType.Imagination, zoneId))
                return;

            await AddProgressAndCheck(zoneId);
        }

        public async Task ReportWorldEnter(ZoneId zoneId)
        {
            if (!IsValid(RacingTaskType.EnterWorld, zoneId))
                return;

            await AddProgressAndCheck(zoneId);
        }

        private async Task AddProgressAndCheck(float value)
        {
            AddProgress(value);
            if (Completed)
                await CheckMissionCompletedAsync();
        }

        private bool IsCompleted()
        {
            switch ((RacingTaskType)Parameters[0])
            {
                case RacingTaskType.Rank:
                case RacingTaskType.Laptime:
                case RacingTaskType.Racetime:
                    return CurrentProgress > 0;

                default:
                    return base.Completed;
            }
        }
        
        private bool IsValid(RacingTaskType taskType, int target) =>
            Parameters[0] == (int)taskType && Targets.Contains(target);
    }
}
