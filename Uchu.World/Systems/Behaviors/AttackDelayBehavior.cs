using System.IO;
using System.Threading.Tasks;
using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.World.Systems.Behaviors
{
    public class AttackDelayBehaviorExecutionParameters : BehaviorExecutionParameters
    {
        public bool ServerSide { get; set; }
        public uint Handle { get; set; }
        public BehaviorExecutionParameters Parameters { get; set; }
        
        public byte[] SyncStream { get; set; }

        public AttackDelayBehaviorExecutionParameters(ExecutionContext context, ExecutionBranchContext branchContext) 
            : base(context, branchContext)
        {
        }
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
        
        protected override void DeserializeStart(BitReader reader, AttackDelayBehaviorExecutionParameters parameters)
        {
            parameters.Handle = reader.Read<uint>();
            for (var i = 0; i < Intervals; i++)
                parameters.RegisterHandle<AttackDelayBehaviorExecutionParameters>(parameters.Handle, DeserializeSync,
                    ExecuteSync);
        }

        protected override void DeserializeSync(BitReader reader, AttackDelayBehaviorExecutionParameters parameters)
        {
            parameters.Parameters = Action.DeserializeStart(reader, parameters.Context, parameters.BranchContext);
        }

        protected override void SerializeStart(BitWriter writer, AttackDelayBehaviorExecutionParameters parameters)
        {
            parameters.Handle = parameters.NpcContext.Associate.GetComponent<SkillComponent>().ClaimSyncId();
            writer.Write(parameters.Handle);
        }

        protected override void SerializeSync(BitWriter writer, AttackDelayBehaviorExecutionParameters parameters)
        {
            parameters.Parameters = Action.SerializeStart(writer, parameters.NpcContext,
                parameters.BranchContext);
            parameters.SyncStream = (writer.BaseStream as MemoryStream)?.ToArray();
            parameters.ServerSide = true;
        }

        protected override void ExecuteSync(AttackDelayBehaviorExecutionParameters parameters)
        {
            if (parameters.ServerSide)
            {
                parameters.Schedule( () =>
                {
                    parameters.NpcContext.Sync(parameters.SyncStream, parameters.Handle);
                    Action.ExecuteStart(parameters.Parameters);
                }, Delay);
            }
            else
            {
                Action.ExecuteStart(parameters.Parameters);
            }
        }
    }
}