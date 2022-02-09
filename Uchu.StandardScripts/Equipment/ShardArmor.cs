using System.Numerics;
using System.Threading.Tasks;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.Equipment
{
    /// <summary>
    /// Native implementation of scripts/equipmenttriggers/shardarmor.lua
    /// </summary>
    [ScriptName("shardarmor.lua")]
    public class ShardArmor : Coil
    {
        public ShardArmor(GameObject gameObject) : base(gameObject)
        {
            SkillID = 1249;
            CoilThreshold = 5;
            Ready = true;
        }
    }
}