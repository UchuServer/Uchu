using System.Threading.Tasks;
using Uchu.Core;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.VentureExplorer
{
    [ZoneSpecific(1000)]
    public class RocketBuildCheck: NativeScript
    {
        public override Task LoadAsync()
        {
            Listen(Zone.OnPlayerLoad, player =>
            {
                var inventory = player.GetComponent<InventoryManagerComponent>();
                
                Listen(inventory.OnLotAdded, async (lot, count) =>
                {
                    Logger.Information($"PICKUP: {lot}");

                    if (lot != Lot.ModularRocket) return;

                    Logger.Information($"UPDATING FOR: {lot}");
                    
                    var questInventory = player.GetComponent<MissionInventoryComponent>();

                    await questInventory.FlagAsync(44);
                });
                
                return Task.CompletedTask;
            });
            
            return Task.CompletedTask;
        }
    }
}