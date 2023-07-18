using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.General.DeathPlane
{
    /// <summary>
    /// Native implementation of scripts/ai/ns/ns_pp_01/l_ns_pp_01_teleport.lua
    /// </summary>
    [ScriptName("l_ns_pp_01_teleport.lua")]
    public class PropertyDeathPlane : ObjectScript
    {
        /// <summary>
        /// Creates the object script.
        /// </summary>
        /// <param name="gameObject">Game object to control with the script.</param>
        public PropertyDeathPlane(GameObject gameObject) : base(gameObject)
        {
            var physics = gameObject.GetComponent<PhysicsComponent>();
            if (physics == default) return;
            Listen(physics.OnEnter, other =>
            {
                if (other.GameObject is not Player player) return;
                var teleportObject = this.GetGroup("Teleport")[0];
                player.Teleport(teleportObject.Transform.Position, ignore: false);
            });
        }
    }
}