using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.AvantGardens
{
    /// <summary>
    /// Native implementation of scripts/ai/ag/l_ag_saluting_npcs.lua
    /// </summary>
    [ScriptName("l_ag_saluting_npcs.lua")]
    public class SalutingNpc : ObjectScript
    {
        /// <summary>
        /// Creates the object script.
        /// </summary>
        /// <param name="gameObject">Game object to control with the script.</param>
        public SalutingNpc(GameObject gameObject) : base(gameObject)
        {
            // Listen for players saluting the object.
            Listen(gameObject.OnEmoteReceived, (emoteId, player) =>
            {
                if (emoteId == 356)
                {
                    this.PlayAnimation("salutePlayer");
                }
            });
            
            // Listen to players interacting with the object.
            Listen(gameObject.OnInteract, player =>
            {
                this.PlayAnimation("salutePlayer");
            });
        }
    }
}