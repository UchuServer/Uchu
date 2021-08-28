using System.Numerics;
using System.Threading.Tasks;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.Equipment
{
    /// <summary>
    /// Native implementation of scripts/equipmenttriggers/sentinelspaceranger3.lua
    /// </summary>
    [ScriptName("sentinelspaceranger3.lua")]
    public class SpaceRanger3 : SkillSetTriggerTemplate
    {
        public SpaceRanger3(GameObject gameObject) : base(gameObject)
        {
            if (gameObject is Item item)
            {
                SkillID = 1103;
                SetID = 12;
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