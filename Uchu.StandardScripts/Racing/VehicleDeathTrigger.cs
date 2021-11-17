using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.Racing;

[ScriptName("L_ACT_VEHICLE_DEATH_TRIGGER.lua")]
public class VehicleDeathTrigger : ObjectScript
{
    /// <summary>
    /// Script to kill cars that enter death triggers
    /// </summary>
    /// <param name="gameObject"></param>
    public VehicleDeathTrigger(GameObject gameObject) : base(gameObject)
    {
        if (!gameObject.TryGetComponent<PhysicsComponent>(out var physicsComponent))
            return;

        if (!gameObject.Zone.ZoneControlObject.TryGetComponent<RacingControlComponent>(out var racingControlComponent))
            return;


        Listen(physicsComponent.OnEnter, other =>
        {
            if (other.GameObject is not Player player)
                return;

            racingControlComponent.OnPlayerRequestDie(player);
        });
    }
}
