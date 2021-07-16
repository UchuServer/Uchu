using Uchu.World;
using Uchu.World.Scripting.Native;
using Uchu.World.Social;

namespace Uchu.StandardScripts.General
{

    [ScriptName("ScriptComponent_1088_script_name__removed")]
    public class Mailbox : ObjectScript
    {
        /// <summary>
        /// Creates the object script.
        /// </summary>
        /// <param name="gameObject">Game object to control with the script.</param>
        public Mailbox(GameObject gameObject) : base(gameObject)
        {
            Listen(gameObject.OnInteract, async player =>
            {
                await player.OpenMailboxGuiAsync();
            });
        }

        /// <summary>
        /// Callback that is run once with the first GameObject created.
        /// </summary>
        public override void CompleteOnce()
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
        }
    }
}
