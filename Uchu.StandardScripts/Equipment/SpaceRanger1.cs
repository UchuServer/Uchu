using System.Numerics;
using System.Threading.Tasks;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.Equipment
{
    /// <summary>
    /// Native implementation of scripts/equipmenttriggers/sentinelspaceranger1.lua
    /// </summary>
    [ScriptName("sentinelspaceranger1.lua")]
    public class SpaceRanger1 : SkillSetTriggerTemplate
    {
        public SpaceRanger1(GameObject gameObject) : base(gameObject)
        {
            if (gameObject is Item item)
            {
                SkillID = 1101;
                SetID = 10;
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