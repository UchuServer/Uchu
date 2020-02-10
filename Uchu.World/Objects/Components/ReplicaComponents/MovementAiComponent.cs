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

                Speed *= 3;

                Origin = Transform.Position;
            });

            Listen(OnTick, () =>
            {
                if (!Enabled) return;

                if (BaseCombatAiComponent == default)
                {
                    BaseCombatAiComponent = GameObject.GetComponent<BaseCombatAiComponent>();
                }

                if (ControllablePhysicsComponent == default)
                {
                    ControllablePhysicsComponent = GameObject.GetComponent<ControllablePhysicsComponent>();
                }
                
                if (BaseCombatAiComponent.Cooldown > 0 && BaseCombatAiComponent.AbilityDowntime) return;
                
                var targets = BaseCombatAiComponent.SeekValidTargets().Where(target =>
                {
                    var transform = target.Transform;

                    var distance = Vector3.Distance(transform.Position, Transform.Position);

                    return distance <= 50;
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

                targetPosition.Y = position.Y;

                position = position.MoveTowards(targetPosition, Speed, out var delta);

                Transform.Position = position;

                var prev = ControllablePhysicsComponent.Velocity;
                
                ControllablePhysicsComponent.Velocity = delta;

                ControllablePhysicsComponent.HasVelocity = delta != prev;

                Transform.LookAt(targetPosition);
            });
        }
        
        public override void Construct(BitWriter writer)
        {
        }

        public override void Serialize(BitWriter writer)
        {
        }
    }
}