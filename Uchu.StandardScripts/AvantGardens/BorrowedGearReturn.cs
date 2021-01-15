using System.Threading.Tasks;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.AvantGardens
{
    [ZoneSpecific(1100)]
    public class BorrowedGearReturn : NativeScript
    {
        public override Task LoadAsync()
        {
            Listen(Zone.OnPlayerLoad, player =>
            {
                if (!player.TryGetComponent<MissionInventoryComponent>(out var missionInventory)) return;

                Listen(missionInventory.OnCompleteMission, instance =>
                {
                    if (instance.MissionId != 313) return;
                    
                    if (!player.TryGetComponent<InventoryManagerComponent>(out var inventoryManager)) return;

                    // Remove mission item
                    inventoryManager.RemoveAll(14549);

                    // Remove faction gear
                    inventoryManager.RemoveAll(14359);
                    inventoryManager.RemoveAll(14321);
                    inventoryManager.RemoveAll(14353);
                    inventoryManager.RemoveAll(14315);
                });
            });

            return Task.CompletedTask;
        }
    }
}