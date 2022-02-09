using System.Numerics;
using System.Threading.Tasks;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.General
{
    /// <summary>
    /// Native implementation of scripts/equipmentscripts/fountainofimagination.lua
    /// </summary>
    [ScriptName("fountainofimagination.lua")]
    public class FountainOfImagination : SpawnPowerupsOnTimerThenDie
    {
        /// <summary>
        /// Creates the object script.
        /// </summary>
        /// <param name="gameObject">Game object to control with the script.</param>
        public FountainOfImagination(GameObject gameObject) : base(gameObject)
        {
            SpawnPowerups(gameObject, 6, 30, 1.5f, 30, 5, 935);
        }
    }
}