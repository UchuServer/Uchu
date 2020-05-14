using System.Threading.Tasks;

namespace Uchu.World.Behaviors
{
    public class AttackDelayBehavior : BehaviorBase
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.AttackDelay;
        
        public int Delay { get; set; }
        
        public BehaviorBase Action { get; set; }
        
        public int Intervals { get; set; }
        
        public override async Task BuildAsync()
        {
            Action = await GetBehavior("action");

            Intervals = await GetParameter<int>("num_intervals");

            if (Intervals == 0)
            {
                Intervals = 1;
            }
            
            var delay = await GetParameter("delay");
            
            if (delay.Value == null) return;

            Delay = (int) (delay.Value * 1000);
        }

        public override async Task ExecuteAsync(ExecutionContext context, ExecutionBranchContext branchContext)
        {
            await base.ExecuteAsync(context, branchContext);

            var handle = context.Reader.Read<uint>();

            for (var i = 0; i < Intervals; i++)
            {
                RegisterHandle(handle, context, branchContext);
            }
        }

        public override async Task SyncAsync(ExecutionContext context, ExecutionBranchContext branchContext)
        {
            await Action.ExecuteAsync(context, branchContext);
        }

        public override Task CalculateAsync(NpcExecutionContext context, ExecutionBranchContext branchContext)
        {
            var syncId = context.Associate.GetComponent<SkillComponent>().ClaimSyncId();

            context.Writer.Write(syncId);

            if (branchContext.Target is Player player)
            {
                player.SendChatMessage($"Delay. [{context.SkillSyncId}] [{syncId}]");
            }

            Task.Run(async () =>
            {
                await Task.Delay(Delay);

                context = context.Copy();
                
                await Action.CalculateAsync(context, branchContext);

                context.Sync(syncId);
                
                if (branchContext.Target is Player sPlayer)
                {
                    sPlayer.SendChatMessage($"Sync. [{context.SkillSyncId}] [{syncId}]");
                }
            });
            
            return Task.CompletedTask;
        }
    }
}