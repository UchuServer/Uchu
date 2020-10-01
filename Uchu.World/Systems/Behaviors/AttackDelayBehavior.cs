using System.Threading.Tasks;
using Uchu.Core;

namespace Uchu.World.Systems.Behaviors
{
    public class AttackDelayBehaviorExecutionParameters : BehaviorExecutionParameters
    {
        public uint Handle { get; set; }
        public BehaviorExecutionParameters Parameters { get; set; }
    }
    
    public class AttackDelayBehavior : BehaviorBase<AttackDelayBehaviorExecutionParameters>
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.AttackDelay;

        private int Delay { get; set; }

        private BehaviorBase Action { get; set; }

        private int Intervals { get; set; }
        
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
        
        protected override void DeserializeStart(AttackDelayBehaviorExecutionParameters behaviorExecutionParameters)
        {
            behaviorExecutionParameters.Handle = behaviorExecutionParameters.Context.Reader.Read<uint>();
            for (var i = 0; i < Intervals; i++)
            {
                RegisterHandle(behaviorExecutionParameters.Handle, behaviorExecutionParameters);
            }
        }

        protected override void DeserializeSync(AttackDelayBehaviorExecutionParameters behaviorExecutionParameters)
        {
            behaviorExecutionParameters.Parameters = Action.DeserializeStart(behaviorExecutionParameters.Context,
                behaviorExecutionParameters.BranchContext);
        }

        protected override async Task ExecuteSync(AttackDelayBehaviorExecutionParameters behaviorExecutionParameters)
        {
            await Action.ExecuteStart(behaviorExecutionParameters.Parameters);
        }

        public override Task SerializeStart(NpcExecutionContext context, ExecutionBranchContext branchContext)
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
                
                await Action.SerializeStart(context, branchContext);

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