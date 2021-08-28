using System.Numerics;
using System.Threading.Tasks;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.Equipment
{
    /// <summary>
    /// Native implementation of scripts/equipmenttriggers/sentinelsamurai2.lua
    /// </summary>
    [ScriptName("sentinelsamurai2.lua")]
    public class Samurai2 : SkillSetTriggerTemplate
    {
        public Samurai2(GameObject gameObject) : base(gameObject)
        {
            if (gameObject is Item item)
            {
                SkillID = 563;
                SetID = 14;
                ItemsRequired = 4;
                CooldownTime = 0;
                Listen(item.Owner.GetComponent<DestroyableComponent>().OnArmorChanged, (newA, delta) => 
                {
                    if (newA < 1)
                    {
                        Process(item);
                    }
                });
            }
        }
    }
}