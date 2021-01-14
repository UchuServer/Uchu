using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RakDotNet.IO;
using Uchu.Physics;
using Uchu.World;
using Uchu.World.Scripting.Native;
using Uchu.World.Systems.Behaviors;

namespace Uchu.StandardScripts.General
{
    public class ImaginePowerupSpawner : NativeScript
    {
        private const int SkillID = 13;
        public override Task LoadAsync()
        {
            foreach (var gameObject in Zone.GameObjects.Where(g => g.Lot == 1656))
            {
                Mount(gameObject);
            }

            Listen(Zone.OnObject, obj =>
            {
                if (!(obj is GameObject gameObject)) return;

                if (gameObject.Lot == 1656)
                {
                    Mount(gameObject);
                }
            });

            return Task.CompletedTask;
        }

        private void Mount(GameObject gameObject)
        {
            var physics = gameObject.AddComponent<PhysicsComponent>();

            var size = Vector3.Divide(Vector3.One, 3);

            var physicsObject = BoxBody.Create(
                gameObject.Zone.Simulation,
                gameObject.Transform.Position,
                gameObject.Transform.Rotation,
                size
            );
            
            physics.SetPhysics(physicsObject);

            Listen(physics.OnEnter,  other =>
            {
                if (!(other.GameObject is Player player)) return;
                if (!gameObject.GetComponent<DestructibleComponent>().Alive) return;
                
                gameObject.PlayFX("", "pickup");
                player.GetComponent<DestroyableComponent>().Imagination += 1;

                Task.Run(async delegate
                {
                    await Task.Delay(633);
                    Zone.BroadcastMessage(new DieMessage
                    {
                        Associate = gameObject,
                        Killer = player,
                        KillType = 1
                    });
                    gameObject.GetComponent<DestructibleComponent>().Alive = false;
                });
            });
        }
    }
}