using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Uchu.Core;

namespace Uchu.World.Systems.Behaviors
{
    public class AirMovementBehaviorExecutionParameters : BehaviorExecutionParameters
    {
        public uint Handle { get; set; }
        public BehaviorBase Action { get; set; }
        public BehaviorExecutionParameters ActionParameters { get; set; }
        public AirMovementBehaviorExecutionParameters(ExecutionContext context, ExecutionBranchContext branchContext) 
            : base(context, branchContext)
        {
        }
    }
    
    public class AirMovementBehavior : BehaviorBase<AirMovementBehaviorExecutionParameters>
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.AirMovement;

        private BehaviorBase GroundAction { get; set; }
        
        public override async Task BuildAsync()
        {
            GroundAction = await GetBehavior("ground_action");
        }

        protected override void DeserializeStart(AirMovementBehaviorExecutionParameters parameters)
        {
            parameters.Handle = parameters.Context.Reader.Read<uint>();
            parameters.RegisterHandle<AirMovementBehaviorExecutionParameters>(parameters.Handle, DeserializeSync, ExecuteSync);
        }

        protected override async void DeserializeSync(AirMovementBehaviorExecutionParameters parameters)
        {
            var behaviorId = parameters.Context.Reader.Read<uint>();
            parameters.Action = await GetBehavior(behaviorId);

            var targetId = parameters.Context.Reader.Read<ulong>();
            parameters.Context.Associate.Zone.TryGetGameObject((long)targetId,
                out var target);
            
            parameters.ActionParameters = parameters.Action.DeserializeStart(
                parameters.Context, new ExecutionBranchContext
                {
                    Duration = parameters.BranchContext.Duration,
                    Target = target ?? parameters.BranchContext.Target
                });
        }

        protected override void ExecuteSync(AirMovementBehaviorExecutionParameters parameters)
        {
            parameters.Action.ExecuteStart(parameters.ActionParameters);
        }
    }
}