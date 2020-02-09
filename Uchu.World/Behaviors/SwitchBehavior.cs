using System.Threading.Tasks;

namespace Uchu.World.Behaviors
{
    public class SwitchBehavior : BehaviorBase
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.Switch;

        public BehaviorBase Action { get; set; }
        
        public int Imagination { get; set; }
        
        public bool IsEnemyFaction { get; set; }
        
        public BehaviorBase ActionFalse { get; set; }
        
        public BehaviorBase ActionTrue { get; set; }
        
        public override async Task BuildAsync()
        {
            Action = await GetBehavior("action");
            ActionFalse = await GetBehavior("action_false");
            ActionTrue = await GetBehavior("action_true");

            Imagination = await GetParameter<int>("imagination");

            IsEnemyFaction = (await GetParameter("isEnemyFaction"))?.Value > 0;
        }

        public override async Task ExecuteAsync(ExecutionContext context, ExecutionBranchContext branchContext)
        {
            await base.ExecuteAsync(context, branchContext);
            
            var handle = context.Reader.Read<uint>();

            context.Writer.Write(handle);

            RegisterHandle(handle, context, branchContext);

            var state = true;

            if (Imagination > 0 || !IsEnemyFaction)
            {
                state = context.Reader.ReadBit();

                context.Writer.WriteBit(state);
            }

            if (state)
            {
                await ActionTrue.ExecuteAsync(context, branchContext);
            }
            else
            {
                await ActionFalse.ExecuteAsync(context, branchContext);
            }
        }

        public override async Task CalculateAsync(NpcExecutionContext context, ExecutionBranchContext branchContext)
        {
            var syncId = context.Associate.GetComponent<SkillComponent>().ClaimSyncId();

            context.Writer.Write(syncId);

            // TODO

            var state = true;
            
            if (Imagination > 0 || !IsEnemyFaction)
            {
                state = branchContext.Target != default && context.Alive;
                
                context.Writer.WriteBit(state);
            }

            if (state)
            {
                await ActionTrue.CalculateAsync(context, branchContext);
            }
            else
            {
                await ActionFalse.CalculateAsync(context, branchContext);
            }

            var _ = Task.Run(async () =>
            {
                context = context.Copy();
                
                await Action.CalculateAsync(context, branchContext);

                context.Sync(syncId);
            });
        }

        public override async Task SyncAsync(ExecutionContext context, ExecutionBranchContext branchContext)
        {
            await Action.ExecuteAsync(context, branchContext);
        }
    }
}