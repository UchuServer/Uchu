using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.Racing;

[ScriptName("FV_RACE_SMASH_EGG_IMAGINE_SERVER.lua")]
public class ForbiddenValleyEggs : ObjectScript
{
    public ForbiddenValleyEggs(GameObject gameObject) : base(gameObject)
    {
        Listen(gameObject.GetComponent<DestructibleComponent>().OnSmashed, (killer, lootOwner) =>
        {
            PlayFXEffect("onHit", "onHit", 4102);
            if (killer is not Player player)
                return;

            var car = player.GetComponent<CharacterComponent>().VehicleObject;
            if (car is null)
                return;
            
            var skillComponent = car.GetComponent<SkillComponent>();
            skillComponent.CalculateSkillAsync(586, car);
        });
    }
}
