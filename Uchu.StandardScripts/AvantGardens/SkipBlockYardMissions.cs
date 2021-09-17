using System.Linq;
using System.Threading.Tasks;
using Uchu.Core.Resources;
using Uchu.World;
using Uchu.World.Scripting.Native;
using Uchu.World.Social;

namespace Uchu.StandardScripts.AvantGardens
{
    [ZoneSpecific(1100)]
    public class SkipBlockYardMissions : NativeScript
    {
        public override Task LoadAsync()
        {
            var crashHelmut = Zone.GameObjects.First(g => g.Lot == Lot.CrashHelmutNpc);
            Listen(crashHelmut.GetComponent<MissionGiverComponent>().OnMissionOk, async tuple =>
            {
                var (missionId, _, _, player) = tuple;

                // 'go to block yard' mission
                if (missionId != (int) MissionId.BackupBulwark)
                    return;

                // small delay to make sure the mission has been added to the player's mission inventory and started
                await Task.Delay(1000);

                // complete 'go to block yard' mission
                var missionInventory = player.GetComponent<MissionInventoryComponent>();
                var bulwarkMission = missionInventory.GetMission(MissionId.BackupBulwark);
                await bulwarkMission.CompleteAsync();

                // complete 'smash spider queen' mission
                var arachnophobiaMission = await missionInventory.AddMissionAsync((int) MissionId.Arachnophobia, player);
                await arachnophobiaMission.StartAsync();
                await arachnophobiaMission.CompleteAsync();

                // start 'talk to sky lane' mission
                var skyLaneMission = await missionInventory.AddMissionAsync((int) MissionId.CheckInwithSkyLane, player);
                await skyLaneMission.StartAsync();

                // show pop-up to player
                await UiHelper.AnnouncementAsync((Player) player, "Completed Missions",
                    "Block Yard is currently not implemented. The missions have been completed for you.");
            });

            return Task.CompletedTask;
        }
    }
}
