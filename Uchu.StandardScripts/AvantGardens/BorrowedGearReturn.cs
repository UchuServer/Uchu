using System.Threading.Tasks;
using Uchu.Core.Client;
using Uchu.Core.Resources;
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
                if (!player.TryGetComponent<MissionInventoryComponent>(out var missionInventory))
                    return;

                Listen(missionInventory.OnCompleteMission, instance =>
                {
                    if (instance.MissionId != (int)MissionId.SpiderFighter) 
                        return;
                    
                    if (!player.TryGetComponent<InventoryManagerComponent>(out var inventoryManager))
                        return;

                    // Remove mission item
                    inventoryManager.RemoveAllAsync(14549);

                    // Remove faction gear
                    inventoryManager.RemoveAllAsync(14359);
                    inventoryManager.RemoveAllAsync(14321);
                    inventoryManager.RemoveAllAsync(14353);
                    inventoryManager.RemoveAllAsync(14315);
                });
            });

            return Task.CompletedTask;
        }
    }
}