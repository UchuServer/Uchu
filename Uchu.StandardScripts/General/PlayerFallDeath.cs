using System.Threading.Tasks;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.General
{
    /// <summary>
    /// Native implementation of scripts/ai/gf/l_player_fall_death.lua
    /// </summary>
    [ScriptName("l_player_fall_death.lua")]
    public class PlayerFallDeath : ObjectScript
    {
        /// <summary>
        /// Creates the object script.
        /// </summary>
        /// <param name="gameObject">Game object to control with the script.</param>
        public PlayerFallDeath(GameObject gameObject) : base(gameObject)
        {
            var physics = gameObject.GetComponent<PhysicsComponent>();
            if (physics == default) return;
            Listen(physics.OnEnter, other =>
            {
                if (!(other.GameObject is Player player)) return;
                Task.Run(async () =>
                {
                    await Task.Delay(2000);
                    await player.GetComponent<DestructibleComponent>().SmashAsync(gameObject);
                });
            });
        }
    }
}