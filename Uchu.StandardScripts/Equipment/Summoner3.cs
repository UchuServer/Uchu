using System.Numerics;
using System.Threading.Tasks;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.Equipment
{
    /// <summary>
    /// Native implementation of scripts/equipmenttriggers/assemblysummoner3.lua
    /// </summary>
    [ScriptName("assemblysummoner3.lua")]
    public class Summoner3 : SkillSetTriggerTemplate
    {
        public Summoner3(GameObject gameObject) : base(gameObject)
        {
            if (gameObject is Item item)
            {
                SkillID = 582;
                SetID = 30;
                ItemsRequired = 4;
                CooldownTime = 11;
                Listen(item.Owner.GetComponent<DestroyableComponent>().OnImaginationChanged, (newI, delta) => 
                {
                    if (newI < 1)
                    {
                        Process(item);
                    }
                });
            }
        }
    }
}