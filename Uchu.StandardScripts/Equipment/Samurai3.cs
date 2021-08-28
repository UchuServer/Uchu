using System.Numerics;
using System.Threading.Tasks;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.Equipment
{
    /// <summary>
    /// Native implementation of scripts/equipmenttriggers/sentinelsamurai3.lua
    /// </summary>
    [ScriptName("sentinelsamurai3.lua")]
    public class Samurai3 : SkillSetTriggerTemplate
    {
        public Samurai3(GameObject gameObject) : base(gameObject)
        {
            if (gameObject is Item item)
            {
                SkillID = 564;
                SetID = 15;
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