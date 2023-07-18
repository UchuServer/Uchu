using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.Racing;

[ScriptName("vehicle_death_trigger_water_server.lua")]
public class VehicleDeathTriggerWater : ObjectScript
{
    /// <summary>
    /// Script to kill cars that enter death triggers
    /// </summary>
    /// <param name="gameObject"></param>
    public VehicleDeathTriggerWater(GameObject gameObject) : base(gameObject)
    {
        if (!gameObject.TryGetComponent<PhysicsComponent>(out var physicsComponent)) return;
        if (!gameObject.Zone.ZoneControlObject.TryGetComponent<RacingControlComponent>(out var racingControlComponent)) return;
        Listen(physicsComponent.OnEnter, other =>
        {
            if (other.GameObject is not Player player) return;
            var car = player.GetComponent<CharacterComponent>().VehicleObject;
            Zone.BroadcastMessage(new DieMessage
            {
                Associate = car,
                Killer = GameObject,
                KillType = KillType.Violent,
                DeathType = "death_water",
                ClientDeath = true,
                SpawnLoot = false,
            });
            racingControlComponent.OnPlayerRequestDie(player);
        });
    }
}