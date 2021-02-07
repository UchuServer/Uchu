using System.Threading.Tasks;
using Uchu.Core;
using Uchu.Core.Resources;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.AvantGardens
{
    [ZoneSpecific(1100)]
    public class LaserRampageReturn : NativeScript
    {
        public override Task LoadAsync()
        {
            Listen(Zone.OnPlayerLoad, player =>
            {
                if (!player.TryGetComponent<MissionInventoryComponent>(out var missionInventory))
                    return;

                Listen(missionInventory.OnCompleteMission, instance =>
                {
                    if (instance.MissionId != (int)MissionId.LaserRampage && instance.MissionId != (int)MissionId.WreckingCrew)
                        return;

                    if (!player.TryGetComponent<InventoryManagerComponent>(out var inventoryManager))
                        return;

                    // Remove hamster wheels and flashlights
                    inventoryManager.RemoveAllAsync(14555);
                    inventoryManager.RemoveAllAsync(14556);
                });
            });

            return Task.CompletedTask;
        }
    }
}