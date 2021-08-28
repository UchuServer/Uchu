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
            if (gameObject is Item item)
            {
                SkillID = 1488;
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