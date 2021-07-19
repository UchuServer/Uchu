using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.VentureExplorer
{
    [ScriptName("l_ag_ship_player_death_trigger.lua")]
    public class DeathTrigger : ObjectScript
    {
        public DeathTrigger(GameObject gameObject) : base(gameObject)
        {
            if (!gameObject.TryGetComponent<PhysicsComponent>(out var physics))
                return;
            Listen(physics.OnEnter, other =>
            {
                if (other.GameObject is Player player)
                {
                    player.GetComponent<DestructibleComponent>()
                        .SmashAsync(gameObject, animation: "electro-shock-death");
                }
            });
        }
    }
}
