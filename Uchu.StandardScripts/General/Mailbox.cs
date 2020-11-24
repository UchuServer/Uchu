using System.Linq;
using System.Threading.Tasks;
using Uchu.World;
using Uchu.World.Scripting.Native;
using Uchu.World.Social;

namespace Uchu.StandardScripts.General
{
    public class Mailbox : NativeScript
    {
        public override Task LoadAsync()
        {
            Listen(Zone.OnObject, (obj) => {
                if ((obj as GameObject).Lot == 3964)
                {
                    Listen((obj as GameObject).OnInteract, async player =>
                    {
                        await player.OpenMailboxGuiAsync();
                    });
                }
            });

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