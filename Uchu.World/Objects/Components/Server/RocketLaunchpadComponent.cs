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
            if (!player.GetComponent<InventoryManager>().TryFindItem(Lot.ModularRocket, out var rocket))
            {
                Logger.Error($"{player} attempted to launch a rocket without having one in their inventory.");
                    
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
            
            using var ctx = new UchuContext();

            var character = await ctx.Characters.FirstAsync(c => c.CharacterId == player.ObjectId);

            character.LandingByRocket = true;
            
            character.Rocket = ((LegoDataList) rocket.Settings["assemblyPartLOTs"]).ToString(";") + ";";
            
            await ctx.SaveChangesAsync();
        }
    }
}