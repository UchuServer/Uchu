using System.Linq;
using System.Threading.Tasks;
using Uchu.Core;

namespace Uchu.World.Systems.Behaviors
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

            if (HitAction.BehaviorId != 0 || HitActionEnemy.BehaviorId != 0 || HitActionFaction.BehaviorId != 0)
            {
                var handle = context.Reader.Read<uint>();
                RegisterHandle(handle, context, branchContext);
                Logger.Debug("ForceMovementBehavior");
            }
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