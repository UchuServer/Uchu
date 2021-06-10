using System.Linq;
using System.Threading.Tasks;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.NinjagoMonastery
{
    [ZoneSpecific(2000)]
    public class GoToNpcBodge : NativeScript
    {
        public override Task LoadAsync()
        {
            // Okay look this is a really stupid script but the cdclient is messed up.
            // Check the lu-explorer links I left here. The GoToNpc tasks have
            // entirely unrelated targets. (These targets aren't used for any other
            // GoToNpc tasks, so it should be fine to just call GoToNpc for them.)

            // https://lu.lcdruniverse.org/explorer/missions/1797
            const int senseiWuLot = 13789;
            const int senseiWuMission = 1797;
            const int senseiWuMissionTarget = 13879; // NT_ASSEMBLY PET CONSOLE ???
            var senseiWu = Zone.GameObjects.First(g => g.Lot == senseiWuLot);
            Listen(senseiWu.OnInteract, player =>
            {
                if (!player.TryGetComponent<MissionInventoryComponent>(out var missionInventory))
                    return;
                if (!missionInventory.HasActive(senseiWuMission))
                    return;
                missionInventory.GoToNpcAsync(senseiWuMissionTarget);
                missionInventory.GetMission(senseiWuMission).CompleteAsync();
            });

            // https://lu.lcdruniverse.org/explorer/missions/1798
            const int jayLot = 13792;
            const int jayMission = 1798;
            const int jayMissionTarget = 13798; // some guard in a cave?
            var jay = Zone.GameObjects.First(g => g.Lot == jayLot);
            Listen(jay.OnInteract, player =>
            {
                if (!player.TryGetComponent<MissionInventoryComponent>(out var missionInventory))
                    return;
                if (!missionInventory.HasActive(jayMission))
                    return;
                missionInventory.GoToNpcAsync(jayMissionTarget);
                missionInventory.GetMission(jayMission).CompleteAsync();
            });

            return Task.CompletedTask;
        }
    }
}
