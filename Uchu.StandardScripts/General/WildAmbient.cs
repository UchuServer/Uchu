using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.General
{
    /// <summary>
    /// Native implementation of scripts/ai/wild/l_wild_ambients.lua
    /// </summary>
    [ScriptName("l_wild_ambients.lua")]
    public class WildAmbient : ObjectScript
    {
        public WildAmbient(GameObject gameObject) : base(gameObject)
        {
            Listen(gameObject.OnInteract, player =>
            {
                gameObject.Animate("interact");
            });
        }
    }
}
