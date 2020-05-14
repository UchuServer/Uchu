using System.Numerics;
using System.Threading.Tasks;
using Uchu.Core;

namespace Uchu.World.Behaviors
{
    public class ProjectileAttackBehavior : BehaviorBase
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.ProjectileAttack;
        
        public int ProjectileCount { get; set; }
        
        public Lot ProjectileLot { get; set; }
        
        public float ProjectileSpeed { get; set; }
        
        public float MaxDistance { get; set; }
        
        public float TrackRadius { get; set; }

        public override async Task BuildAsync()
        {
            ProjectileCount = await GetParameter<int>("spread_count");

            ProjectileLot = await GetParameter<int>("LOT_ID");

            ProjectileSpeed = await GetParameter<float>("projectile_speed");

            MaxDistance = await GetParameter<float>("max_distance");

            TrackRadius = await GetParameter<float>("track_radius"); // ???
        }

        public override async Task ExecuteAsync(ExecutionContext context, ExecutionBranchContext branchContext)
        {
            await base.ExecuteAsync(context, branchContext);
            
            var target = context.Reader.ReadGameObject(context.Associate.Zone);

            var count = ProjectileCount == 0 ? 1 : ProjectileCount;
            
            for (var i = 0; i < count; i++)
            {
                StartProjectile(context, target);
            }
        }

        public override async Task CalculateAsync(NpcExecutionContext context, ExecutionBranchContext branchContext)
        {
            context.Writer.Write(branchContext.Target);
            
            var count = ProjectileCount == 0 ? 1 : ProjectileCount;
            
            for (var i = 0; i < count; i++)
            {
                CalculateProjectile(context, branchContext.Target);
            }
        }
        
        private void CalculateProjectile(ExecutionContext context, GameObject target)
        {
            context.Associate.Transform.LookAt(target.Transform.Position);
            
            if (target is Player player)
            {
                player.SendChatMessage("You are a projectile target!");
            }

            var projectileId = ObjectId.Standalone;

            context.Writer.Write(projectileId);
            
            var projectile = Object.Instantiate<Projectile>(context.Associate.Zone);

            projectile.Owner = context.Associate;
            projectile.ClientObjectId = projectileId;
            projectile.Target = target;
            projectile.Lot = ProjectileLot;
            projectile.Destination = target.Transform.Position;
            projectile.RadiusCheck = TrackRadius;
            projectile.MaxDistance = MaxDistance;

            Object.Start(projectile);

            Task.Run(async () =>
            {
                var distance = Vector3.Distance(context.Associate.Transform.Position, target.Transform.Position);

                var time = (int) (distance / (double) ProjectileSpeed) * 1000;

                await Task.Delay(time);

                await projectile.CalculateImpactAsync(target);
            });
        }

        private void StartProjectile(ExecutionContext context, GameObject target)
        {
            var projectileId = context.Reader.Read<long>();

            var projectile = Object.Instantiate<Projectile>(context.Associate.Zone);

            projectile.Owner = context.Associate;
            projectile.ClientObjectId = projectileId;
            projectile.Target = target;
            projectile.Lot = ProjectileLot;
            projectile.Destination = target.Transform.Position;
            projectile.RadiusCheck = TrackRadius;
            projectile.MaxDistance = MaxDistance;
            
            Object.Start(projectile);
        }
    }
}