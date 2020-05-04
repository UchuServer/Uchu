using System.Threading.Tasks;
using Uchu.Core;

namespace Uchu.World
{
    [ServerComponent(Id = ComponentId.PackageComponent)]
    public class ItemPackageComponent : Component
    {
        private ItemPackageComponent()
        {
            Listen(OnStart, () =>
            {
                if (!(GameObject is Item item))
                {
                    Logger.Error("Component mismatch");

                    Destroy(this);
                    
                    return;
                }

                Listen(item.OnConsumed, ConsumeAsync);
            });
        }

        private async Task ConsumeAsync()
        {
            if (!(GameObject is Item item)) return;
            
            var container = GameObject.AddComponent<LootContainerComponent>();

            await container.CollectDetailsAsync();

            var manager = item.Inventory.ManagerComponent;
            
            foreach (var lot in container.GenerateLootYields())
            {
                await manager.AddItemAsync(lot, 1);
            }
        }
    }
}