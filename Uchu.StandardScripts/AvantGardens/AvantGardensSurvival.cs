using Uchu.StandardScripts.Base;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.AvantGardens
{
    /// <summary>
    /// Native implementation of scripts/zone/ag/l_zone_ag_survival.lua
    /// </summary>
    [ScriptName("l_zone_ag_survival.lua")]
    public class AvantGardensSurvival : BaseSurvivalGame
    {
        /// <summary>
        /// Creates the object script.
        /// </summary>
        /// <param name="gameObject">Game object to control with the script.</param>
        public AvantGardensSurvival(GameObject gameObject) : base(gameObject)
        {
            
        }
    }
}