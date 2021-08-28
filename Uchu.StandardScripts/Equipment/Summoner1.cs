using System.Numerics;
using System.Threading.Tasks;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.Equipment
{
    /// <summary>
    /// Native implementation of scripts/equipmenttriggers/assemblysummoner1.lua
    /// </summary>
    [ScriptName("assemblysummoner1.lua")]
    public class Summoner1 : SkillSetTriggerTemplate
    {
        public Summoner1(GameObject gameObject) : base(gameObject)
        {
            if (gameObject is Item item)
            {
                SkillID = 394;
                SetID = 28;
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