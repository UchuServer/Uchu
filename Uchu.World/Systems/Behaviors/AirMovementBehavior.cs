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
    }
    
    public class AirMovementBehavior : BehaviorBase<AirMovementBehaviorExecutionParameters>
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.AirMovement;
        
        public override Task BuildAsync()
        {
            return Task.CompletedTask;
        }

        protected override void DeserializeStart(AirMovementBehaviorExecutionParameters behaviorExecutionParameters)
        {
            behaviorExecutionParameters.Handle = behaviorExecutionParameters.Context.Reader.Read<uint>();
            RegisterHandle(behaviorExecutionParameters.Handle, behaviorExecutionParameters);
        }

        protected override async void DeserializeSync(AirMovementBehaviorExecutionParameters behaviorExecutionParameters)
        {
            behaviorExecutionParameters.Action = await GetBehavior(
                behaviorExecutionParameters.Context.Reader.Read<uint>());
            
            var targetId = behaviorExecutionParameters.Context.Reader.Read<ulong>();
            behaviorExecutionParameters.Context.Associate.Zone.TryGetGameObject((long)targetId,
                out var target);
            
            behaviorExecutionParameters.ActionParameters = behaviorExecutionParameters.Action.DeserializeStart(
                behaviorExecutionParameters.Context, new ExecutionBranchContext
                {
                    Duration = behaviorExecutionParameters.BranchContext.Duration,
                    Target = target
                });
        }

        protected override async Task ExecuteSync(AirMovementBehaviorExecutionParameters behaviorExecutionParameters)
        {
            await behaviorExecutionParameters.Action.ExecuteStart(behaviorExecutionParameters.ActionParameters);
        }
    }
}