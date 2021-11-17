using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.Racing;

[ScriptName("FV_RACE_SMASH_EGG_IMAGINE_SERVER.lua")]
public class ForbiddenValleyEggs : ObjectScript
{
    public ForbiddenValleyEggs(GameObject gameObject) : base(gameObject)
    {
        this.Listen(gameObject.GetComponent<DestructibleComponent>().OnSmashed, (killer, lootOwner) =>
        {
            if (killer is not Player player)
                return;

            var car = player.GetComponent<CharacterComponent>().VehicleObject;
            if (car is null)
                return;

            // it's meant to be skill 586...
            var destroyableComponent = car.GetComponent<DestroyableComponent>();
            destroyableComponent.Imagination += 10;
        });
    }
}
