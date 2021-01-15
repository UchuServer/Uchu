using System;
using System.Threading.Tasks;
using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.World.Systems.Behaviors
{
    public class MovementSwitchBehaviorExecutionParameters : BehaviorExecutionParameters
    {
        public BehaviorExecutionParameters BehaviorExecutionParameters { get; set; }
        public BehaviorBase ToExecute { get; set; }
        public MovementType MovementType { get; set; }

        public MovementSwitchBehaviorExecutionParameters(ExecutionContext context, ExecutionBranchContext branchContext)
            : base(context, branchContext)
        {
        }
    }
    public class MovementSwitchBehavior : BehaviorBase<MovementSwitchBehaviorExecutionParameters>
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.MovementSwitch;

        private BehaviorBase GroundBehavior { get; set; }
        private BehaviorBase JumpBehavior { get; set; }
        private BehaviorBase FallingBehavior { get; set; }
        private BehaviorBase DoubleJumpBehavior { get; set; }
        private BehaviorBase AirBehavior { get; set; }
        private BehaviorBase JetpackBehavior { get; set; }
        private BehaviorBase MovingBehavior { get; set; }

        public override void Build()
        {
            GroundBehavior = GetBehavior("ground_action");
            JumpBehavior = GetBehavior("jump_action");
            FallingBehavior = GetBehavior("falling_action");
            DoubleJumpBehavior = GetBehavior("double_jump_action");
            AirBehavior = GetBehavior("air_action");
            JetpackBehavior = GetBehavior("ground_action");
            MovingBehavior = GetBehavior("moving_action");
        }

        protected override void DeserializeStart(BitReader reader, MovementSwitchBehaviorExecutionParameters parameters)
        {
            parameters.MovementType = (MovementType)reader.Read<uint>();
            switch (parameters.MovementType)
            {
                case MovementType.Moving:
                    // Should be handled as ground behavior
                    parameters.ToExecute = GroundBehavior;
                    break;
                case MovementType.Ground:
                    parameters.ToExecute = GroundBehavior;
                    break;
                case MovementType.Jump:
                    parameters.ToExecute = JumpBehavior;
                    break;
                case MovementType.Falling:
                    parameters.ToExecute = FallingBehavior;
                    break;
                case MovementType.DoubleJump:
                    parameters.ToExecute = DoubleJumpBehavior;
                    break;
                case MovementType.Air:
                    parameters.ToExecute = AirBehavior;
                    break;
                case MovementType.Jetpack:
                    parameters.ToExecute = JetpackBehavior;
                    break;
                case MovementType.Unknown:
                    // Should be handled as ground behavior
                    parameters.ToExecute = GroundBehavior;
                    break;
                default:
                    throw new Exception($"Invalid {nameof(MovementType)}! Got {parameters.MovementType}!");
            }
            
            parameters.BehaviorExecutionParameters = parameters.ToExecute.DeserializeStart(reader, parameters.Context, 
                parameters.BranchContext);
        }

        protected override void ExecuteStart(MovementSwitchBehaviorExecutionParameters parameters)
        {
            parameters.ToExecute?.ExecuteStart(parameters.BehaviorExecutionParameters);
        }
    }
}