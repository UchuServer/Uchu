using System.Linq;
using System.Threading.Tasks;
using Uchu.World;
using Uchu.World.Scripting.Native;
using Uchu.World.Social;

namespace Uchu.StandardScripts.General
{

    [ScriptName("ScriptComponent_1088_script_name__removed")]
    public class MailboxObjectScript : ObjectScript
    {
        /// <summary>
        /// Creates the object script.
        /// </summary>
        /// <param name="gameObject">Game object to control with the script.</param>
        public MailboxObjectScript(GameObject gameObject) : base(gameObject)
        {
            Listen(gameObject.OnInteract, async player =>
            {
                await player.OpenMailboxGuiAsync();
            });
        }
    }
    
    public class MailboxEventScript : NativeScript
    {
        /// <summary>
        /// Loads the script.
        /// </summary>
        public override Task LoadAsync()
        {
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
