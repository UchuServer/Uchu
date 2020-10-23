using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Uchu.Core.Client;
using Uchu.World;
using Uchu.World.Scripting.Native;
using Uchu.World.Social;

namespace Uchu.StandardScripts.General
{
    public class NotImplemented : NativeScript
    {
        public override Task LoadAsync()
        {
            Listen(Zone.OnPlayerLoad, player => {
                Listen(player.OnRespondToMission, AvantGardensSurvival);
                Listen(player.OnRespondToMission, BlockYard);
            });

            return Task.CompletedTask;
        }

        private async Task AvantGardensSurvival(int missionID, GameObject playerObject, Lot rewardItem)
        {
            if (missionID != 479) return;

            MissionInventoryComponent MissionInventory = playerObject.GetComponent<MissionInventoryComponent>();
            await MissionInventory.CompleteMissionAsync(479);

            await UiHelper.AnnouncementAsync(playerObject as Player, "Unfinished", "Avant Gardens Survival is not complete so we have skipped it for you");
        }

        private async Task BlockYard(int missionID, GameObject playerObject, Lot rewardItem)
        {
            if (missionID != 377) return;

            MissionInventoryComponent MissionInventory = playerObject.GetComponent<MissionInventoryComponent>();
            await MissionInventory.CompleteMissionAsync(377);
            await MissionInventory.CompleteMissionAsync(1950);
            await MissionInventory.CompleteMissionAsync(768);
            await MissionInventory.CompleteMissionAsync(870);
            await MissionInventory.CompleteMissionAsync(871);
            await MissionInventory.CompleteMissionAsync(891);

            await UiHelper.AnnouncementAsync(playerObject as Player, "Unfinished", "Block Yard is not complete so we have completed the worlds missions for you");
        }
    }
}