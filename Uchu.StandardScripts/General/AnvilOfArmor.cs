using System.Numerics;
using System.Threading.Tasks;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.General
{
    /// <summary>
    /// Native implementation of scripts/equipmentscripts/anvilofarmor.lua
    /// </summary>
    [ScriptName("anvilofarmor.lua")]
    public class AnvilOfArmor : SpawnPowerupsOnTimerThenDie
    {
        /// <summary>
        /// Creates the object script.
        /// </summary>
        /// <param name="gameObject">Game object to control with the script.</param>
        public AnvilOfArmor(GameObject gameObject) : base(gameObject)
        {
            SpawnPowerups(gameObject, 8, 25, 1.5f, 25, 4, 6431);
        }
    }
}