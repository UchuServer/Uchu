using System.Numerics;
using System.Threading.Tasks;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.Equipment
{
    /// <summary>
    /// Native implementation of scripts/equipmenttriggers/assemblyinventor1.lua
    /// </summary>
    [ScriptName("assemblyinventor1.lua")]
    public class Inventor1 : SkillSetTriggerTemplate
    {
        public Inventor1(GameObject gameObject) : base(gameObject)
        {
            if (gameObject is Item item)
            {
                SkillID = 394;
                SetID = 25;
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