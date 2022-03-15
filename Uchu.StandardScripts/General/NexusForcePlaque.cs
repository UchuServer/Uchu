using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.General
{
    /// <summary>
    /// Script to set flags for story plaques (https://lu.lcdruniverse.org/explorer/objects/8139) the player uses
    /// </summary>
    [ScriptName("ScriptComponent_1054_script_name__removed")]
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
            if (gameObject.Settings.TryGetValue("altFlagID", out var altFlagId))
                flag = (int) altFlagId;
                
            // Set the flag for players who interact with the plaque.
            Listen(gameObject.OnInteract, async player =>
            {
                if (player.TryGetComponent<CharacterComponent>(out var character))
                {
                    await character.SetFlagAsync(flag, true);
                    player.Message(new FireEventClientSideMessage
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
