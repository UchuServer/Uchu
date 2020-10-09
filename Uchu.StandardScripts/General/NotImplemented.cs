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
            });

            return Task.CompletedTask;
        }

        private async Task AvantGardensSurvival(int missionID, GameObject playerObject, Lot rewardItem)
        {
            if (missionID != 479) return;

            MissionInventoryComponent MissionInventory = playerObject.GetComponent<MissionInventoryComponent>();
            await MissionInventory.CompleteMissionAsync(479);

            await UiHelper.AnnouncementAsync(playerObject as Player, "Unfinished", "Avant gardens is not complete so we have skipped it for you");
        }
    }
}