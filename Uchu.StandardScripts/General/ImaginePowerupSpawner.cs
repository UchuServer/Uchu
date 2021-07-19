using System.Numerics;
using Uchu.Physics;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.General
{
    /// <summary>
    /// Native implementation of scripts/ai/spec/l_special_imagine-powerup-spawner.lua
    /// </summary>
    [ScriptName("l_special_imagine-powerup-spawner.lua")]
    public class ImaginePowerupSpawner : ObjectScript
    {
        /// <summary>
        /// Creates the object script.
        /// </summary>
        /// <param name="gameObject">Game object to control with the script.</param>
        public ImaginePowerupSpawner(GameObject gameObject) : base(gameObject)
        {
            // Create the physics.
            var physics = gameObject.AddComponent<PhysicsComponent>();
            var size = Vector3.Divide(Vector3.One, 3);
            var physicsObject = BoxBody.Create(
                gameObject.Zone.Simulation,
                gameObject.Transform.Position,
                gameObject.Transform.Rotation,
                size
            );
            physics.SetPhysics(physicsObject);
            
            // Connect the player entering the power-up.
            Listen(physics.OnEnter, other =>
            {
                // Return if the collided object isn't a player.
                if (!(other.GameObject is Player player)) return;
                if (!gameObject.GetComponent<DestructibleComponent>().Alive) return;

                // Play the effect, add the imagination, and destroy the power-up.
                this.PlayFXEffect("", "pickup");
                player.GetComponent<DestroyableComponent>().Imagination += 1;
                this.Die(player);
            });
        }
    }
}