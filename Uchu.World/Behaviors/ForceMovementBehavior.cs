using System.Linq;
using System.Threading.Tasks;

namespace Uchu.World.Behaviors
{
    public class ForceMovementBehavior : BehaviorBase
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.ForceMovement;
        
        public BehaviorBase HitAction { get; set; }
        
        public BehaviorBase HitActionEnemy { get; set; }
        
        public BehaviorBase HitActionFaction { get; set; }
        
        public override async Task BuildAsync()
        {
            HitAction = await GetBehavior("hit_action");
            HitActionEnemy = await GetBehavior("hit_action_enemy");
            HitActionFaction = await GetBehavior("hit_action_faction");
        }

        public override async Task ExecuteAsync(ExecutionContext context, ExecutionBranchContext branchContext)
        {
            await base.ExecuteAsync(context, branchContext);

            var array = new[] {HitAction, HitActionEnemy, HitActionFaction};
            
            if (array.All(b => b?.BehaviorId == 0)) return;

            var handle = context.Reader.Read<uint>();
            
            RegisterHandle(handle, context, branchContext);
        }

        public override async Task SyncAsync(ExecutionContext context, ExecutionBranchContext branchContext)
        {
            var actionId = context.Reader.Read<uint>();

            var action = await GetBehavior(actionId);

            var id = context.Reader.Read<ulong>();

            context.Associate.Zone.TryGetGameObject((long) id, out var target);

            var branch = new ExecutionBranchContext(target)
            {
                Duration = branchContext.Duration
            };

            await action.ExecuteAsync(context, branch);
        }
    }
}