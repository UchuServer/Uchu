using System.Threading.Tasks;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.General.DeathPlane
{
    /// <summary>
    /// Native implementation of scripts/ai/act/l_act_player_death_trigger.lua
    /// </summary>
    [ScriptName("l_act_player_death_trigger.lua")]
    public class DeathPlane : ObjectScript
    {
        /// <summary>
        /// Creates the object script.
        /// </summary>
        /// <param name="gameObject">Game object to control with the script.</param>
        public DeathPlane(GameObject gameObject) : base(gameObject)
        {
            var physics = gameObject.GetComponent<PhysicsComponent>();
            if (physics == default) return;
            Listen(physics.OnEnter, other =>
            {
                if (other.GameObject is not Player player) return;
                Task.Run(async () =>
                {
                    await player.GetComponent<DestructibleComponent>().SmashAsync(gameObject);
                });
            });
        }
    }
}