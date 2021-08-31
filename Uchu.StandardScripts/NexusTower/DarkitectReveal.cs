using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.NexusTower
{
    [ScriptName("ScriptComponent_1556_script_name__removed")]
    public class DarkitectReveal : ObjectScript
    {
        /// <summary>
        /// Creates the object script.
        /// </summary>
        /// <param name="gameObject">Game object to control with the script.</param>
        public DarkitectReveal(GameObject gameObject) : base(gameObject)
        {
            // Listen for players interacting with this object
            Listen(gameObject.OnInteract, player =>
            {
                // The client script will handle everything after it receives this message.
                player.Message(new NotifyClientObjectMessage
                {
                    Name = "reveal",
                    ParamObj = player,
                    Associate = gameObject,
                });

                Zone.Schedule(() =>
                {
                    // Set player's health to 1, as shown in the cinematic.
                    // Wait 2 seconds before doing this to ensure it won't be visible before the cinematic starts.
                    var stats = player.GetComponent<DestroyableComponent>();
                    stats.Health = 1;
                    stats.Armor = 0;

                    // Set flag for https://lu-explorer.web.app/missions/1295
                    var character = player.GetComponent<CharacterComponent>();
                    character.SetFlagAsync(1911, true);
                }, 2000);
            });
        }
    }
}
