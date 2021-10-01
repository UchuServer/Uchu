using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.General
{
    /// <summary>
    /// Script to set flags for binoculars (e.g. https://lu.lcdruniverse.org/explorer/objects/6700) the player uses
    /// </summary>
    [ScriptName("ScriptComponent_1002_script_name__removed")] // Binoculars, id 6700
    [ScriptName("ScriptComponent_952_script_name__removed")] // AG - Spaceship Binoculars, id 6842
    [ScriptName("ScriptComponent_975_script_name__removed")] // GF - Binoculars, id 6958
    [ScriptName("ScriptComponent_1020_script_name__removed")] // PR - Binoculars, id 7607
    [ScriptName("ScriptComponent_1021_script_name__removed")] // NS - Binoculars, id 7608
    [ScriptName("ScriptComponent_1335_script_name__removed")] // FB - Binoculars, id 12306
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
