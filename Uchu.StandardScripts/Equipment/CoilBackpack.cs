using System.Numerics;
using System.Threading.Tasks;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.Equipment
{
    /// <summary>
    /// Native implementation of scripts/equipmenttriggers/coilbackpack.lua
    /// </summary>
    [ScriptName("coilbackpack.lua")]
    public class CoilBackpack : Coil
    {
        public CoilBackpack(GameObject gameObject) : base(gameObject)
        {
            SkillID = 1001;
            CoilThreshold = 5;
            Ready = true;
        }
    }
}