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
            if (gameObject is Item item)
            {
                SkillID = 1249;
                CoilThreshold = 5;
                Listen(item.Owner.GetComponent<DestroyableComponent>().OnHealthChanged, (newH, delta) => 
                {
                    if (delta < 0){
                        Process(item);
                    }
                });
                Listen(item.Owner.GetComponent<DestroyableComponent>().OnArmorChanged, (newA, delta) => 
                {
                    if (delta < 0){
                        Process(item);
                    }
                });
            }
        }
    }
}