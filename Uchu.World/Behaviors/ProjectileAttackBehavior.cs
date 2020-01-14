using System.Threading.Tasks;

namespace Uchu.World.Behaviors
{
    public class ProjectileAttackBehavior : BehaviorBase
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.ProjectileAttack;
        
        public int ProjectileCount { get; set; }
        
        public Lot ProjectileLot { get; set; }

        public override async Task BuildAsync()
        {
            ProjectileCount = await GetParameter<int>("spread_count");

            ProjectileLot = await GetParameter<int>("LOT_ID");
        }

        public override async Task ExecuteAsync(ExecutionContext context, ExecutionBranchContext branchContext)
        {
            await base.ExecuteAsync(context, branchContext);
            
            var target = context.Reader.ReadGameObject(context.Associate.Zone);

            context.Writer.Write<long>(target);

            ((Player) context.Associate)?.SendChatMessage($"{ProjectileCount} projectiles.");
            
            if (ProjectileCount == 0)
            {
                StartProjectile(context, target);
            }
            else
            {
                for (var i = 0; i < ProjectileCount; i++)
                {
                    StartProjectile(context, target);
                }
            }
        }

        private void StartProjectile(ExecutionContext context, GameObject target)
        {
            var projectileId = context.Reader.Read<long>();

            context.Writer.Write(projectileId);

            var projectile = Object.Instantiate<Projectile>(context.Associate.Zone);

            projectile.Owner = context.Associate;
            projectile.ClientObjectId = projectileId;
            projectile.Target = target;
            projectile.Lot = ProjectileLot;
                
            ((Player) context.Associate)?.SendChatMessage($"Start PROJ: [{projectile.Lot}] {projectile.ClientObjectId} -> {projectile.Target}");

            Object.Start(projectile);
        }
    }
}