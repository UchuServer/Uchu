using System.Threading.Tasks;
using Uchu.Core;
using Uchu.World;
using Uchu.World.Scripting;

namespace Uchu.StandardScripts.VentureExplorer
{
    [ZoneSpecific(ZoneId.VentureExplorer)]
    public class RocketBuildCheck: Script
    {
        public override Task LoadAsync()
        {
            Listen(Zone.OnPlayerLoad, player =>
            {
                var inventory = player.GetComponent<InventoryManagerComponent>();
                
                Listen(inventory.OnLotAdded, (lot, count) =>
                {
                    Logger.Information($"PICKUP: {lot}");
                    
                    if (lot != Lot.ModularRocket) return Task.CompletedTask;

                    Logger.Information($"UPDATING FOR: {lot}");
                    
                    var questInventory = player.GetComponent<MissionInventoryComponent>();

                    questInventory.UpdateObjectTask(MissionTaskType.Flag, 44);
                    
                    return Task.CompletedTask;
                });
                
                return Task.CompletedTask;
            });
            
            return Task.CompletedTask;
        }
    }
}