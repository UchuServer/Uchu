using System.Numerics;
using System.Threading.Tasks;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.Equipment
{
    /// <summary>
    /// Native implementation of scripts/equipmenttriggers/sentinelspaceranger2.lua
    /// </summary>
    [ScriptName("sentinelspaceranger2.lua")]
    public class SpaceRanger2 : SkillSetTriggerTemplate
    {
        public SpaceRanger2(GameObject gameObject) : base(gameObject)
        {
            if (gameObject is Item item)
            {
                SkillID = 1102;
                SetID = 11;
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