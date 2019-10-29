using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Uchu.Core;
using Uchu.World.Collections;

namespace Uchu.World
{
    [ServerComponent(Id = ComponentId.RocketLaunchComponent)]
    public class RocketLaunchpadComponent : Component
    {
        public RocketLaunchpadComponent()
        {
            OnStart.AddListener(() =>
            {
                GameObject.OnInteract.AddListener(async player =>
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
                Logger.Error($"Could not find a valid rocket for {player}", true);
                
                return;
            }

            player.GetComponent<InventoryComponent>().EquipItem(rocket, true);
            
            player.Message(new ChangeObjectWorldStateMessage
            {
                Associate = rocket,
                State = ObjectWorldState.Attached
            });
            
            player.Message(new FireClientEventMessage
            {
                Associate = GameObject,
                Arguments = "RocketEquipped",
                Target = rocket,
                Sender = player
            });

            await using var ctx = new UchuContext();

            var character = await ctx.Characters.FirstAsync(c => c.CharacterId == player.ObjectId);

            character.LandingByRocket = true;
            
            character.Rocket = ((LegoDataList) rocket.Settings["assemblyPartLOTs"]).ToString(";") + ";";
            
            await ctx.SaveChangesAsync();
        }
    }
}