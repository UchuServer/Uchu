using System.Linq;
using System.Threading.Tasks;
using Uchu.World.Scripting.Native;
using Uchu.World.Social;

namespace Uchu.StandardScripts.General
{
    public class Mailbox : NativeScript
    {
        public override Task LoadAsync()
        {
            foreach (var gameObject in Zone.GameObjects.Where(g => g.Lot == 3964))
            {
                Listen(gameObject.OnInteract, async player =>
                {
                    await player.OpenMailboxGuiAsync();
                });
            }

            Listen(Zone.OnPlayerLoad, player =>
            {
                Listen(player.OnFireServerEvent, async (messagename, message) =>
                {
                    if (messagename == "toggleMail")
                    {
                        await UiHelper.ToggleAsync(player, "ToggleMail", false);
                    }

                });
            });

            return Task.CompletedTask;
        }
    }
}
