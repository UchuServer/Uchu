using System.Numerics;
using System.Threading.Tasks;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.Equipment
{
    /// <summary>
    /// Native implementation of scripts/equipmentscripts/buccaneervaliantship.lua
    /// </summary>
    [ScriptName("buccaneervaliantship.lua")]
    public class BroadsiderSchooner : ObjectScript
    {
        public BroadsiderSchooner(GameObject gameObject) : base(gameObject)
        {
            Task.Run(async () => 
            {
                await Task.Delay(1000);
                var skillComponent = gameObject.GetComponent<SkillComponent>();
                skillComponent.CalculateSkillAsync(982);
                //ForceMovementBehavior isn't finished yet, but the script works as intended.
                await Task.Delay(1100);
                gameObject.GetComponent<DestructibleComponent>().SmashAsync(gameObject);
            });
        }
    }
}