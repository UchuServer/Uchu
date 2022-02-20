using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.Racing;

[ScriptName("RACE_IMAGINE_CRATE_SERVER.lua")]
public class ImaginationCrate : ObjectScript
{
    public ImaginationCrate(GameObject gameObject) : base(gameObject)
    {
        Listen(gameObject.GetComponent<DestructibleComponent>().OnSmashed, (killer, lootOwner) =>
        {
            if (killer is not Player player)
                return;

            var car = player.GetComponent<CharacterComponent>().VehicleObject;
            if (car is null)
                return;
            
            var skillComponent = car.GetComponent<SkillComponent>();
            skillComponent.CalculateSkillAsync(585, car);
        });
    }
}
