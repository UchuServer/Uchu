using System.Threading.Tasks;

namespace Uchu.World.Systems.Behaviors
{
    public class AirMovementBehavior : BehaviorBase
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.AirMovement;
        
        public override Task BuildAsync()
        {
            return Task.CompletedTask;
        }

        public override async Task ExecuteAsync(ExecutionContext context, ExecutionBranchContext branchContext)
        {
            await base.ExecuteAsync(context, branchContext);

            var handle = context.Reader.Read<uint>();

            RegisterHandle(handle, context, branchContext);
        }

        public override async Task SyncAsync(ExecutionContext context, ExecutionBranchContext branchContext)
        {
            await base.ExecuteAsync(context, branchContext);
            
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