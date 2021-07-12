using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.Base
{
    /// <summary>
    /// Native implementation of scripts/ai/act/l_act_generic_activity_mgr.lua
    /// </summary>
    public class GenericActivityManager : ObjectScript
    {
        /// <summary>
        /// Creates the object script.
        /// </summary>
        /// <param name="gameObject">Game object to control with the script.</param>
        public GenericActivityManager(GameObject gameObject) : base(gameObject)
        {
            
        }
    }
}