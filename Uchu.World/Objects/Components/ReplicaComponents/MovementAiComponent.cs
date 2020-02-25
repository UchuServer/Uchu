using System.Linq;
using System.Numerics;
using Microsoft.EntityFrameworkCore;
using RakDotNet.IO;
using Uchu.Core.Client;

namespace Uchu.World
{
    public class MovementAiComponent : ReplicaComponent
    {
        public override ComponentId Id => ComponentId.MovementAIComponent;
        
        private MovementAIComponent ClientInfo { get; set; }
        
        private BaseCombatAiComponent BaseCombatAiComponent { get; set; }
        
        private ControllablePhysicsComponent ControllablePhysicsComponent { get; set; }
        
        private float Speed { get; set; }
        
        public bool Enabled { get; set; }
        
        private Vector3 Origin { get; set; }
        
        private Vector3 Target { get; set; }
        
        private int Padding { get; set; }

        private Vector3 OldTarget { get; set; }
        
        protected MovementAiComponent()
        {
            Listen(OnStart, async () =>
            {
                await using var ctx = new CdClientContext();

                var info = await ctx.MovementAIComponentTable.FirstOrDefaultAsync(
                    m => m.Id == GameObject.Lot.GetComponentId(ComponentId.MovementAIComponent)
                );

                if (info == default)
                {
                    Destroy(this);
                    
                    return;
                }

                BaseCombatAiComponent = GameObject.GetComponent<BaseCombatAiComponent>();

                ControllablePhysicsComponent = GameObject.GetComponent<ControllablePhysicsComponent>();

                ClientInfo = info;

                Speed = ClientInfo.WanderSpeed ?? 0;

                Speed *= 30;

                Origin = Transform.Position;

                Target = Origin;
            });

            Listen(OnTick, () =>
            {
                if (!Enabled) return;

                CalculatePath();

                var newPosition = Transform.Position.MoveTowards(Target, Speed * Zone.DeltaTime, out var delta);

                if (!(Vector3.Distance(newPosition, Transform.Position) > 1)) return;
                
                ControllablePhysicsComponent.Velocity = delta;

                ControllablePhysicsComponent.HasPosition = true;

                ControllablePhysicsComponent.HasVelocity = delta != Vector3.Zero;

                Transform.Position = newPosition;

                GameObject.Serialize(GameObject);

                Transform.LookAt(newPosition);
            });
        }
        
        public override void Construct(BitWriter writer)
        {
        }

        public override void Serialize(BitWriter writer)
        {
        }

        private void CalculatePath()
        {
            if (BaseCombatAiComponent == default)
            {
                BaseCombatAiComponent = GameObject.GetComponent<BaseCombatAiComponent>();
            }

            if (ControllablePhysicsComponent == default)
            {
                ControllablePhysicsComponent = GameObject.GetComponent<ControllablePhysicsComponent>();
            }
            
            if (BaseCombatAiComponent.AbilityDowntime || BaseCombatAiComponent.SkillEntries.Any(s => s.Cooldown)) return;
            
            var targets = BaseCombatAiComponent.SeekValidTargets().Where(target =>
            {
                var transform = target.Transform;

                var distance = Vector3.Distance(transform.Position, Transform.Position);

                return distance <= 30;
            }).ToList();

            targets.ToList().Sort((g1, g2) =>
            {
                var distance1 = Vector3.Distance(g1.Transform.Position, Transform.Position);
                var distance2 = Vector3.Distance(g2.Transform.Position, Transform.Position);
    
                return (int) (distance2 - distance1);
            });
                
            var position = Transform.Position;
            
            var targetPosition = BaseCombatAiComponent?.Target?.Transform?.Position ??
                                 targets.FirstOrDefault()?.Transform?.Position ??
                                 Origin;
            
            if (targetPosition == Target || Vector3.Distance(position, targetPosition) < 2) return;

            if (targets.Count > 0)
            {
                (targets.First() as Player)?.SendChatMessage("Movement target!");
            }
                
            var path = Zone.NavMeshManager.GeneratePath(position, targetPosition);

            if (targets.Count > 0)
            {
                (targets.First() as Player)?.SendChatMessage($"P: {path.Length}");
            }

            if (path.Length > 2)
            {
                Target = path[2];
            }
        }
    }
}