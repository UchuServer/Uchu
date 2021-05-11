using System.Linq;
using System.Threading.Tasks;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.NinjagoMonastery
{
    [ZoneSpecific(2000)]
    public class TreasureChestMissions : NativeScript
    {
        public override Task LoadAsync()
        {
            // Treasure chests
            var gameObjects = Zone.GameObjects.Where(g => g.Lot == 16295);

            foreach (var gameObject in gameObjects)
            {
                Listen(gameObject.OnInteract, async player =>
                {
                    if (!player.TryGetComponent<MissionInventoryComponent>(out var missionInventory))
                        return;
                    if (!player.TryGetComponent<InventoryManagerComponent>(out var inventory))
                        return;

                    // Earth
                    if (missionInventory.HasActiveForItem(14552))
                    {
                        await inventory.RemoveLotAsync(14486, 1); // Earth Key
                        await inventory.AddLotAsync(14552, 1); // Earth Ninja kit
                    }
                    // Lightning
                    else if (missionInventory.HasActiveForItem(16496))
                    {
                        await inventory.RemoveLotAsync(14489, 1); // Lightning Key
                        await inventory.AddLotAsync(16496, 1); // Lightning Ninja kit
                    }
                    // Ice
                    else if (missionInventory.HasActiveForItem(16498))
                    {
                        await inventory.RemoveLotAsync(14488, 1); // Ice Key
                        await inventory.AddLotAsync(16498, 1); // Ice Ninja kit
                    }
                    // Fire
                    else if (missionInventory.HasActiveForItem(16497))
                    {
                        await inventory.RemoveLotAsync(14487, 1); // Fire Key
                        await inventory.AddLotAsync(16497, 1); // Fire Ninja kit
                    }
                });
            }

            return Task.CompletedTask;
        }
    }
}