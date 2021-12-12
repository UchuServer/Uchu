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

        public async Task ReportRank(int place, ZoneId zoneId)
        {
            if (!IsValid(TaskType.Rank, zoneId))
                return;

            await ReportProgressValue(place, zoneId);
        }

        public async Task ReportLaptime(int laptime, ZoneId zoneId)
        {
            if (!IsValid(TaskType.Laptime, zoneId))
                return;

            await ReportProgressValue(laptime, zoneId);
        }

        public async Task ReportRacetime(int racetime, ZoneId zoneId)
        {
            if (!IsValid(TaskType.Racetime, zoneId))
                return;

            await ReportProgressValue(racetime, zoneId);
        }

        private async Task ReportProgressValue(int value, ZoneId zoneId)
        {
            if (value <= RequiredProgress)
                AddProgress(zoneId);

            if (Completed)
                await CheckMissionCompletedAsync();
        }

        public async Task ReportMissionComplete(int missionId)
        {
            if (!IsValid(TaskType.Achievements, missionId))
                return;

            AddProgress(missionId);

            if (Completed)
                await CheckMissionCompletedAsync();
        }

        public async Task ReportWreck(int wrecks, ZoneId zoneId)
        {
            if (!IsValid(TaskType.Wrecks, zoneId))
                return;

            if (wrecks >= RequiredProgress)
                return;

            AddProgress(zoneId);

            if (Completed)
                await CheckMissionCompletedAsync();
        }

        public async Task ReportImagination(ZoneId zoneId)
        {
            if (!IsValid(TaskType.Imagination, zoneId))
                return;

            AddProgress(zoneId);

            if (Completed)
                await CheckMissionCompletedAsync();
        }

        public async Task ReportWorldEnter(ZoneId zoneId)
        {
            if (!IsValid(TaskType.EnterWorld, zoneId))
                return;

            AddProgress(zoneId);

            if (Completed)
                await CheckMissionCompletedAsync();
        }

        public async Task ReportWin(ZoneId zoneId)
        {
            if (Parameters[0] != (int)TaskType.WinInWorld && Parameters[0] != (int)TaskType.WinInWorlds)
                return;

            if (!Targets.Contains(zoneId))
                return;

            AddProgress(zoneId);

            if (Completed)
                await CheckMissionCompletedAsync();
        }

        public async Task ReportSmash(Lot lot)
        {
            if (!IsValid(TaskType.Smash, lot))
                return;

            AddProgress(lot);

            if (Completed)
                await CheckMissionCompletedAsync();
        }

        private bool IsCompleted()
        {
            switch ((TaskType)Parameters[0])
            {
                case TaskType.Rank:
                case TaskType.Laptime:
                case TaskType.Racetime:
                    return Progress.Contains(Target);

                default:
                    return base.Completed;
            }
        }

        private bool IsValid(TaskType taskType, int target) =>
            Parameters[0] == (int)taskType && Targets.Contains(target);

        private enum TaskType : int
        {
            Rank = 1,
            Laptime = 2,
            Racetime = 3,
            Achievements = 5,
            Wrecks = 10,
            Imagination = 12,
            EnterWorld = 13,
            WinInWorld = 14,
            WinInWorlds = 15,
            Smash = 17
        }
    }
}
