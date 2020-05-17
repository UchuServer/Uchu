using System.Threading.Tasks;

namespace Uchu.World.Systems.Behaviors
{
    public class VerifyBehavior : BehaviorBase
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.Verify;
        
        public BehaviorBase Action { get; set; }
        
        public override async Task BuildAsync()
        {
            Action = await GetBehavior("action");
        }

        public override async Task CalculateAsync(NpcExecutionContext context, ExecutionBranchContext branchContext)
        {
            if (branchContext.Target is Player player)
                player.SendChatMessage($"Verified: [{Action.Id}] {Action.BehaviorId}");

            await Action.CalculateAsync(context, branchContext);
        }
    }
}