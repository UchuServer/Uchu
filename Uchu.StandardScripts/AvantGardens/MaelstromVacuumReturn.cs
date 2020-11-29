using System.Threading.Tasks;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.AvantGardens
{
    [ZoneSpecific(1100)]
    public class MaelstromVacuumReturn : NativeScript
    {
        public override Task LoadAsync()
        {
            Listen(Zone.OnPlayerLoad, player =>
            {
                if (!player.TryGetComponent<MissionInventoryComponent>(out var missionInventory)) return;

                Listen(missionInventory.OnCompleteMission, instance =>
                {
                    if (instance.MissionId != 1849) return;
                    
                    if (!player.TryGetComponent<InventoryManagerComponent>(out var inventoryManager)) return;

                    inventoryManager.RemoveAllAsync(14592);
                });
            });

            return Task.CompletedTask;
        }
    }
}