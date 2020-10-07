using System.Threading.Tasks;
using Uchu.Core;

namespace Uchu.World.Systems.Behaviors
{
    public class AttackDelayBehaviorExecutionParameters : BehaviorExecutionParameters
    {
        public bool WaitAndSync { get; set; }
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
        
        protected override void DeserializeStart(AttackDelayBehaviorExecutionParameters parameters)
        {
            parameters.Handle = parameters.Context.Reader.Read<uint>();
            for (var i = 0; i < Intervals; i++)
                RegisterHandle(parameters.Handle, parameters);
        }

        protected override void DeserializeSync(AttackDelayBehaviorExecutionParameters parameters)
        {
            parameters.Parameters = Action.DeserializeStart(parameters.Context,
                parameters.BranchContext);
        }

        protected override void SerializeStart(AttackDelayBehaviorExecutionParameters parameters)
        {
            parameters.Handle = parameters.NpcContext.Associate.GetComponent<SkillComponent>().ClaimSyncId();
            parameters.NpcContext.Writer.Write(parameters.Handle);
        }

        protected override void SerializeSync(AttackDelayBehaviorExecutionParameters parameters)
        {
            // Copy the context to clear the writer
            parameters.Parameters = Action.SerializeStart(parameters.NpcContext.Copy(),
                parameters.BranchContext);
            parameters.WaitAndSync = true;
        }

        protected override Task ExecuteSync(AttackDelayBehaviorExecutionParameters parameters)
        {
            // Run this async as otherwise the delay can cause the object to be registered as "stuck"
            Task.Run(async () =>
            {
                if (parameters.WaitAndSync)
                    await Task.Delay(Delay);

                await Action.ExecuteStart(parameters.Parameters);

                if (parameters.WaitAndSync)
                    parameters.NpcContext.Sync(parameters.Handle);
            });

            return Task.CompletedTask;
        }
    }
}