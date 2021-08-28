using System.Numerics;
using System.Threading.Tasks;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.General
{
    /// <summary>
    /// Native implementation of scripts/equipmentscripts/sunflower.lua
    /// </summary>
    [ScriptName("sunflower.lua")]
    public class Sunflower : SpawnPowerupsOnTimerThenDie
    {
        /// <summary>
        /// Creates the object script.
        /// </summary>
        /// <param name="gameObject">Game object to control with the script.</param>
        public Sunflower(GameObject gameObject) : base(gameObject)
        {
            SpawnPowerups(gameObject, 6, 5, 1.5f, 30, 4, 11910);
        }
    }
}