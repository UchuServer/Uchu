using System.Numerics;
using System.Threading.Tasks;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.Equipment
{
    /// <summary>
    /// Native implementation of scripts/equipmenttriggers/assemblyinventor3.lua
    /// </summary>
    [ScriptName("assemblyinventor3.lua")]
    public class Inventor3 : SkillSetTriggerTemplate
    {
        public Inventor3(GameObject gameObject) : base(gameObject)
        {
            if (gameObject is Item item)
            {
                SkillID = 582;
                SetID = 27;
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