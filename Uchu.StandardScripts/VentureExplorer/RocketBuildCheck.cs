using System.Threading.Tasks;
using Uchu.Core;
using Uchu.World;
using Uchu.World.Scripting;

namespace StandardScripts.VentureExplorer
{
    [ZoneSpecific(ZoneId.VentureExplorer)]
    public class RocketBuildCheck: Script
    {
        public override Task LoadAsync()
        {
            Zone.OnPlayerLoad.AddListener(player =>
            {
                var inventory = player.GetComponent<InventoryManagerComponent>();
                
                inventory.OnLotAdded.AddListener(async (lot, count) =>
                {
                    Logger.Information($"PICKUP: {lot}");
                    
                    if (lot != Lot.ModularRocket) return;
                    
                    Logger.Information($"UPDATING FOR: {lot}");
                    
                    var questInventory = player.GetComponent<MissionInventoryComponent>();

                    await questInventory.UpdateObjectTaskAsync(MissionTaskType.Flag, 44);
                });
                
                return Task.CompletedTask;
            });
            
            return Task.CompletedTask;
        }
    }
}