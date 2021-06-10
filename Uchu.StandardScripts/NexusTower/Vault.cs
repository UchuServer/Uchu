using System.Linq;
using System.Threading.Tasks;
using Uchu.World;
using Uchu.World.Scripting.Native;
using Uchu.World.Social;

namespace Uchu.StandardScripts.NexusTower
{
    [ZoneSpecific(1900)]
    public class Vault : NativeScript
    {
        public override Task LoadAsync()
        {
            var gameObjects = Zone.GameObjects.Where(g => g.Lot == 13834);

            foreach (var gameObject in gameObjects)
            {
                Listen(gameObject.OnInteract, player =>
                {
                    UiHelper.StateAsync(player, "bank");
                    player.Message(new NotifyClientObjectMessage
                    {
                        Name = "OpenBank",
                        Associate = player,
                    });
                });
            }

            Listen(Zone.OnPlayerLoad, player =>
            {
                Listen(player.OnFireServerEvent, (s, message) =>
                {
                    if (message.Arguments == "ToggleBank")
                    {
                        UiHelper.ToggleAsync(player, "ToggleBank", false);
                        player.Message(new NotifyClientObjectMessage
                        {
                            Name = "CloseBank",
                            Associate = player,
                        });
                    }
                });
            });

            return Task.CompletedTask;
        }
    }
}
