using System.Threading.Tasks;

namespace Uchu.World.Behaviors
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
            var actionId = context.Reader.Read<uint>();

            var action = await GetBehavior(actionId);

            var target = context.Reader.ReadGameObject(context.Associate.Zone);

            await action.ExecuteAsync(context, new ExecutionBranchContext(target));
        }
    }
}