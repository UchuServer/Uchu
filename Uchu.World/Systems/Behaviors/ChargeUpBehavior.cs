using System.Threading.Tasks;
using Uchu.Core;

namespace Uchu.World.Systems.Behaviors
{
    public class ChargeUpBehavior : BehaviorBase
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.ChargeUp;
        
        public BehaviorBase Action { get; set; }
        
        public float MaxDuration { get; set; }
        
        private uint Handle { get; set; }
        
        public override async Task BuildAsync()
        {
            Action = await GetBehavior("action");

            MaxDuration = await GetParameter<float>("max_duration");
        }

        public override Task DeserializeAsync(ExecutionContext context, ExecutionBranchContext branchContext)
        {
            Handle = context.Reader.Read<uint>();
            RegisterHandle(Handle, context, branchContext);
            Logger.Debug("ChargeUpBehavior");
            return base.DeserializeAsync(context, branchContext);
        }

        public override async Task ExecuteAsync(ExecutionContext context, ExecutionBranchContext branchContext)
        {
            await base.ExecuteAsync(context, branchContext);
        }

        public override async Task SyncAsync(ExecutionContext context, ExecutionBranchContext branchContext)
        {
            await base.ExecuteAsync(context, branchContext);
            
            await Action.ExecuteAsync(context, branchContext);
        }
    }
}