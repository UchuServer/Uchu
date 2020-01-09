using System.Threading.Tasks;

namespace Uchu.World.Behaviors
{
    public class SwitchBehavior : BehaviorBase
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.Switch;

        public BehaviorBase Action { get; set; }
        
        public override async Task BuildAsync()
        {
            Action = await GetBehavior("action");
        }

        public override async Task ExecuteAsync(ExecutionContext context, ExecutionBranchContext branchContext)
        {
            await base.ExecuteAsync(context, branchContext);
            
            var handle = context.Reader.Read<uint>();

            RegisterHandle(handle, context, branchContext);
        }

        public override async Task SyncAsync(ExecutionContext context, ExecutionBranchContext branchContext)
        {
            await Action.ExecuteAsync(context, branchContext);
        }
    }
}