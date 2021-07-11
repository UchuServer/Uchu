using System.Numerics;
using System.Threading.Tasks;
using Uchu.Physics;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.AvantGardens
{
    [ScriptName("ScriptComponent_1584_script_name__removed")]
    public class MonumentBirds : ObjectScript
    {
        /// <summary>
        /// Creates the object script.
        /// </summary>
        /// <param name="gameObject">Game object to control with the script.</param>
        public MonumentBirds(GameObject gameObject) : base(gameObject)
        {
            // Add the physics component.
            if (!gameObject.TryGetComponent<DestructibleComponent>(out var destructible)) return;
            var physics = gameObject.AddComponent<PhysicsComponent>();
            var size = Vector3.One * 2;
            var physicsObject = BoxBody.Create(
                gameObject.Zone.Simulation,
                gameObject.Transform.Position,
                gameObject.Transform.Rotation,
                size
            );
            physics.SetPhysics(physicsObject);
            
            // Make the bird fly when a player gets close.
            Listen(physics.OnEnter, other =>
            {
                if (!(other.GameObject is Player player)) return;
                this.PlayAnimation("fly1");
                Task.Run(async () =>
                {
                    await Task.Delay(1000);
                    await destructible.SmashAsync(player, player);
                });
            });
        }
    }
}