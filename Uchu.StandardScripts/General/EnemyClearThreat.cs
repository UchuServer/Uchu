using Uchu.World;
using Uchu.World.Scripting.Native;
using System.Linq;

namespace Uchu.StandardScripts.General
{
    [ScriptName("l_enemy_clear_threat.lua")]
    public class EnemyClearThreat : ObjectScript
    {
        /// <summary>
        /// Creates the object script.
        /// </summary>
        /// <param name="gameObject">Game object to control with the script.</param>
        public EnemyClearThreat(GameObject gameObject) : base(gameObject)
        {
            if (gameObject.TryGetComponent<PhysicsComponent>(out var physicsComponent))
            {
                Listen(physicsComponent.OnEnter, (collider) => {
                    if (collider.GameObject is Player player)
                    {
                        //TODO: When the pathfinding overhaul is complete, add code here to remove aggro of the player who entered from any enemy
                    }
                });
            }
        }
    }
}