using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.World.Systems.Behaviors
{
    public class ProjectileAttackBehaviorExecutionParameters : BehaviorExecutionParameters
    {
        public bool CalculateImpact { get; set; }
        public GameObject Target { get; set; }
        public List<Projectile> Projectiles { get; }

        public ProjectileAttackBehaviorExecutionParameters(ExecutionContext context, ExecutionBranchContext branchContext) 
            : base(context, branchContext)
        {
            CalculateImpact = false;
            Projectiles = new List<Projectile>();
        }
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

        protected override void DeserializeStart(BitReader reader, ProjectileAttackBehaviorExecutionParameters parameters)
        {
            parameters.Target = reader.ReadGameObject(
                parameters.Context.Associate.Zone);
            var count = ProjectileCount == 0 ? 1 : ProjectileCount;

            for (var i = 0; i < count; i++)
            {
                parameters.Projectiles.Add(DeserializeProjectile(reader, parameters));
            }
        }

        /// <summary>
        /// Creates a projectile directed at a target, given a set of parameters
        /// </summary>
        /// <param name="parameters">The behavior parameters to deserialize on</param>
        /// <returns>A projectile</returns>
        private Projectile DeserializeProjectile(BitReader reader, ProjectileAttackBehaviorExecutionParameters parameters)
        {
            var projectileId = reader.Read<long>();
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
        
        protected override void ExecuteStart(ProjectileAttackBehaviorExecutionParameters parameters)
        {
            foreach (var projectile in parameters.Projectiles)
            {
                Object.Start(projectile);
                if (!parameters.CalculateImpact)
                    continue;
                
                // Only server side projectiles have to be computed    
                var distance = Vector3.Distance(parameters.Context.Associate.Transform.Position, 
                    parameters.BranchContext.Target.Transform.Position);
                var time = (int) (distance / (double) ProjectileSpeed) * 1000;

                parameters.Schedule(() =>
                {
                    // Run in the background as this can trigger database IO
                    Task.Factory.StartNew(
                        () => projectile.CalculateImpactAsync(parameters.BranchContext.Target),
                        TaskCreationOptions.LongRunning);
                }, time);
            }
        }
        
        protected override void SerializeStart(BitWriter writer, ProjectileAttackBehaviorExecutionParameters parameters)
        {
            writer.Write(parameters.BranchContext.Target);
            var count = ProjectileCount == 0 ? 1 : ProjectileCount;
            parameters.CalculateImpact = true;

            for (var i = 0; i < count; i++)
            {
                var projectile = SerializeProjectile(writer, parameters);
                parameters.Projectiles.Add(projectile);
            }
        }

        /// <summary>
        /// Creates a projectile shot at a target
        /// </summary>
        /// <param name="parameters">Parameters to write extra data to</param>
        private Projectile SerializeProjectile(BitWriter writer, ProjectileAttackBehaviorExecutionParameters parameters)
        {
            parameters.NpcContext.Associate.Transform.LookAt(parameters.BranchContext.Target.Transform.Position);

            var projectileId = ObjectId.Standalone;
            writer.Write(projectileId);
            
            var projectile = Object.Instantiate<Projectile>(parameters.NpcContext.Associate.Zone);
            projectile.Owner = parameters.NpcContext.Associate;
            projectile.ClientObjectId = projectileId;
            projectile.Target = parameters.BranchContext.Target;
            projectile.Lot = ProjectileLot;
            projectile.Destination = parameters.BranchContext.Target.Transform.Position;
            projectile.RadiusCheck = TrackRadius;
            projectile.MaxDistance = MaxDistance;

            return projectile;
        }
    }
}