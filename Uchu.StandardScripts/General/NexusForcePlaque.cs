using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.General
{
    /// <summary>
    /// Native implementation of scripts/02_client/map/general/l_story_box_interact_client.lua
    /// </summary>
    [ScriptName("l_story_box_interact_client.lua")]
    public class NexusForcePlaque : ObjectScript
    {
        /// <summary>
        /// Creates the object script.
        /// </summary>
        /// <param name="gameObject">Game object to control with the script.</param>
        public NexusForcePlaque(GameObject gameObject) : base(gameObject)
        {
            // Return if there is no story text.
            if (!gameObject.Settings.TryGetValue("storyText", out var text))
                return;

            // Get the flag for the plaque.
            var idString = (string) text;
            var id = int.Parse(idString.Substring(idString.Length - 2));
            var flag = 10000 + Zone.ZoneId + id;
                
            // Set the flag for players who interact with the plaque.
            Listen(gameObject.OnInteract, async player =>
            {
                if (player.TryGetComponent<CharacterComponent>(out var character))
                {
                    await character.SetFlagAsync(flag, true);
                    player.Message(new FireClientEventMessage
                    {
                        Associate = gameObject,
                        Arguments = "achieve",
                        Target = gameObject,
                        Sender = player
                    });
                }
            });
        }
    }
}