using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Uchu.Physics;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.AvantGardens
{
    [ZoneSpecific(1100)]
    public class MonumentBirds : NativeScript
    {
        private const string Animation = "fly1";

        public override Task LoadAsync()
        {
            foreach (var gameObject in Zone.GameObjects.Where(g => g.Lot == 14586))
            {
                Mount(gameObject);
            }

            Listen(Zone.OnObject, obj =>
            {
                if (!(obj is GameObject gameObject)) return;

                if (gameObject.Lot == 14586)
                {
                    Mount(gameObject);
                }
            });

            return Task.CompletedTask;
        }

        public void Mount(GameObject gameObject)
        {
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

            Listen(physics.OnEnter, other =>
            {
                if (!(other.GameObject is Player player)) return;

                gameObject.Animate(Animation);

                Task.Run(async () =>
                {
                    await Task.Delay(1000);

                    await destructible.SmashAsync(player, player);
                });
            });
        }
    }
}