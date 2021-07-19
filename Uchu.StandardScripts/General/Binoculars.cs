using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.General
{
    /// <summary>
    /// Native implementation of scripts/02_client/map/general/l_binoculars_client.lua
    /// </summary>
    [ScriptName("l_binoculars_client.lua")]
    public class Binoculars : ObjectScript
    {
        /// <summary>
        /// Creates the object script.
        /// </summary>
        /// <param name="gameObject">Game object to control with the script.</param>
        public Binoculars(GameObject gameObject) : base(gameObject)
        {
            // Return if there is no number id.
            if (!gameObject.Settings.TryGetValue("number", out var number))
                return;

            // Get the flag for the binoculars.
            var worldId = ((int) Zone.ZoneId).ToString().Substring(0, 2);
            var flag = int.Parse($"{worldId}{number}");
            
            // Set the flag for players who interact with the binoculars.
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