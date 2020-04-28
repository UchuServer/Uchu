using System.Linq;
using System.Threading.Tasks;
using InfectedRose.Lvl;
using Microsoft.EntityFrameworkCore;
using Uchu.Core;

namespace Uchu.World
{
    [ServerComponent(Id = ComponentId.RocketLaunchComponent)]
    public class RocketLaunchpadComponent : Component
    {
        protected RocketLaunchpadComponent()
        {
            Listen(OnStart, () =>
            {
                Listen(GameObject.OnInteract, async player =>
                {
                    await OnInteract(player);
                });
            });
        }

        public async Task OnInteract(Player player)
        {
            var rocket = player.GetComponent<InventoryManagerComponent>()[InventoryType.Models].Items.FirstOrDefault(
                item => item.Lot == Lot.ModularRocket
            );

            if (rocket == default)
            {
                Logger.Error($"Could not find a valid rocket for {player}");
                
                return;
            }

            rocket.WorldState = ObjectWorldState.Attached;
            
            player.Message(new FireClientEventMessage
            {
                Associate = GameObject,
                Arguments = "RocketEquipped",
                Target = rocket,
                Sender = player
            });

            await using var ctx = new UchuContext();

            var character = await ctx.Characters.FirstAsync(c => c.Id == player.Id);

            character.LandingByRocket = true;
            
            await ctx.SaveChangesAsync();
        }
    }
}