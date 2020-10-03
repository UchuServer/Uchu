using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Uchu.Core;

namespace Uchu.World.Systems.Behaviors
{
    public class ProjectileAttackBehaviorExecutionParameters : BehaviorExecutionParameters
    {
        public GameObject Target { get; set; }
        public List<Projectile> Projectiles { get; } = new List<Projectile>();
    }
    public class ProjectileAttackBehavior : BehaviorBase<ProjectileAttackBehaviorExecutionParameters>
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.ProjectileAttack;

        private int ProjectileCount { get; set; }
        private Lot ProjectileLot { get; set; }
        private float ProjectileSpeed { get; set; }
        private float MaxDistance { get; set; }
        private float TrackRadius { get; set; }

        public override async Task BuildAsync()
        {
            ProjectileCount = await GetParameter<int>("spread_count");
            ProjectileLot = await GetParameter<int>("LOT_ID");
            ProjectileSpeed = await GetParameter<float>("projectile_speed");
            MaxDistance = await GetParameter<float>("max_distance");
            TrackRadius = await GetParameter<float>("track_radius"); // ???
        }

        protected override void DeserializeStart(ProjectileAttackBehaviorExecutionParameters parameters)
        {
            parameters.Target = parameters.Context.Reader.ReadGameObject(
                parameters.Context.Associate.Zone);
            var count = ProjectileCount == 0 ? 1 : ProjectileCount;

            for (var i = 0; i < count; i++)
            {
                parameters.Projectiles.Add(DeserializeProjectile(parameters));
            }
        }

        /// <summary>
        /// Creates a projectile directed at a target, given a set of parameters
        /// </summary>
        /// <param name="parameters">The behavior parameters to deserialize on</param>
        /// <returns>A projectile</returns>
        private Projectile DeserializeProjectile(ProjectileAttackBehaviorExecutionParameters parameters)
        {
            var projectileId = parameters.Context.Reader.Read<long>();
            var projectile = Object.Instantiate<Projectile>(parameters.Context.Associate.Zone);

            projectile.Owner = parameters.Context.Associate;
            projectile.ClientObjectId = projectileId;
            projectile.Target = parameters.Target;
            projectile.Lot = ProjectileLot;
            projectile.Destination = parameters.Target.Transform.Position;
            projectile.RadiusCheck = TrackRadius;
            projectile.MaxDistance = MaxDistance;

            return projectile;
        }
        
        protected override Task ExecuteStart(ProjectileAttackBehaviorExecutionParameters parameters)
        {
            foreach (var projectile in parameters.Projectiles)
                Object.Start(projectile);
            return Task.CompletedTask;
        }
        
        protected override void SerializeStart(ProjectileAttackBehaviorExecutionParameters parameters)
        {
            parameters.NpcContext.Writer.Write(parameters.BranchContext.Target);
            var count = ProjectileCount == 0 ? 1 : ProjectileCount;
            
            for (var i = 0; i < count; i++)
            {
                SerializeProjectile(parameters);
            }
        }

        /// <summary>
        /// Creates a projectile shot at a target
        /// </summary>
        /// <param name="parameters">Parameters to write extra data to</param>
        private void SerializeProjectile(ProjectileAttackBehaviorExecutionParameters parameters)
        {
            parameters.NpcContext.Associate.Transform.LookAt(parameters.BranchContext.Target.Transform.Position);

            var projectileId = ObjectId.Standalone;
            parameters.NpcContext.Writer.Write(projectileId);
            
            var projectile = Object.Instantiate<Projectile>(parameters.NpcContext.Associate.Zone);
            projectile.Owner = parameters.NpcContext.Associate;
            projectile.ClientObjectId = projectileId;
            projectile.Target = parameters.BranchContext.Target;
            projectile.Lot = ProjectileLot;
            projectile.Destination = parameters.BranchContext.Target.Transform.Position;
            projectile.RadiusCheck = TrackRadius;
            projectile.MaxDistance = MaxDistance;

            Object.Start(projectile);

            Task.Run(async () =>
            {
                var distance = Vector3.Distance(parameters.Context.Associate.Transform.Position, 
                    parameters.BranchContext.Target.Transform.Position);
                var time = (int) (distance / (double) ProjectileSpeed) * 1000;
                
                await Task.Delay(time);
                await projectile.CalculateImpactAsync(parameters.BranchContext.Target);
            });
        }
    }
}