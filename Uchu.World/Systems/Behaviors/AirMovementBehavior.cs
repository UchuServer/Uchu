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
        public ulong TargetId { get; set; }
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
            
            behaviorExecutionParameters.TargetId = behaviorExecutionParameters.Context.Reader.Read<ulong>();
            
            behaviorExecutionParameters.ActionParameters = behaviorExecutionParameters.Action.DeserializeStart(
                behaviorExecutionParameters.Context, behaviorExecutionParameters.BranchContext);
        }

        protected override async Task ExecuteSync(AirMovementBehaviorExecutionParameters behaviorExecutionParameters)
        {
            behaviorExecutionParameters.ActionParameters.Context.Associate.Zone.TryGetGameObject(
                (long)behaviorExecutionParameters.TargetId, out var target);
            behaviorExecutionParameters.ActionParameters.BranchContext.Target = target;
            
            await behaviorExecutionParameters.Action.ExecuteStart(behaviorExecutionParameters.ActionParameters);
        }
    }
}