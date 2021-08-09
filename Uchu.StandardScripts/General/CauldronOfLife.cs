using System.Numerics;
using System.Threading.Tasks;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.General
{
    /// <summary>
    /// Native implementation of scripts/equipmentscripts/cauldronoflife.lua
    /// </summary>
    [ScriptName("cauldronoflife.lua")]
    public class CauldronOfLife : SpawnPowerupsOnTimerThenDie
    {
        /// <summary>
        /// Creates the object script.
        /// </summary>
        /// <param name="gameObject">Game object to control with the script.</param>
        public CauldronOfLife(GameObject gameObject) : base(gameObject)
        {
            SpawnPowerups(gameObject, 10, 20, 1.5f, 20, 3, 177);
        }
    }
}