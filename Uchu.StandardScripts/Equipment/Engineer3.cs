using System.Numerics;
using System.Threading.Tasks;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.Equipment
{
    /// <summary>
    /// Native implementation of scripts/equipmenttriggers/assemblyengineer3.lua
    /// </summary>
    [ScriptName("assemblyengineer3.lua")]
    public class Engineer3 : SkillSetTriggerTemplate
    {
        public Engineer3(GameObject gameObject) : base(gameObject)
        {
            if (gameObject is Item item)
            {
                SkillID = 582;
                SetID = 4;
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