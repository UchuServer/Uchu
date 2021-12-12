using System.Numerics;
using System.Threading.Tasks;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.Equipment
{
    /// <summary>
    /// Native implementation of scripts/equipmenttriggers/gempack.lua
    /// </summary>
    [ScriptName("gempack.lua")]
    public class GemPack : Coil
    {
        public GemPack(GameObject gameObject) : base(gameObject)
        {
            SkillID = 1488;
            CoilThreshold = 5;
            Ready = true;
        }
    }
}