using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.Racing;

[ScriptName("RACE_IMAGINE_CRATE_SERVER.lua")]
public class ImaginationCrate : ObjectScript
{
    public ImaginationCrate(GameObject gameObject) : base(gameObject)
    {
        this.Listen(gameObject.GetComponent<DestructibleComponent>().OnSmashed, (killer, lootOwner) =>
        {
            if (killer is not Player player)
                return;

            var car = player.GetComponent<CharacterComponent>().VehicleObject;
            if (car is null)
                return;

            // it's meant to be skill 585 but i could not get that to work
            var destroyableComponent = car.GetComponent<DestroyableComponent>();
            destroyableComponent.Imagination += 100;
        });
    }
}
