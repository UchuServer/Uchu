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
            if (gameObject is Item item)
            {
                SkillID = 1001;
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