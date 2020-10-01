using System;
using System.Threading.Tasks;

namespace Uchu.World.Systems.Behaviors
{
    public class MovementSwitchBehaviorExecutionParameters : BehaviorExecutionParameters
    {
        public BehaviorExecutionParameters BehaviorExecutionParameters { get; set; }
        public BehaviorBase ToExecute { get; set; }
        public MovementType MovementType { get; set; }
    }
    public class MovementSwitchBehavior : BehaviorBase<MovementSwitchBehaviorExecutionParameters>
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.MovementSwitch;

        private BehaviorBase GroundBehavior { get; set; }

        private BehaviorBase JumpBehavior { get; set; }

        private BehaviorBase FallingBehavior { get; set; }

        private BehaviorBase DoubleJumpBehavior { get; set; }

        private BehaviorBase JetpackBehavior { get; set; }

        public override async Task BuildAsync()
        {
            GroundBehavior = await GetBehavior("ground_action");
            JumpBehavior = await GetBehavior("jump_action");
            FallingBehavior = await GetBehavior("falling_action");
            DoubleJumpBehavior = await GetBehavior("double_jump_action");
            JetpackBehavior = await GetBehavior("ground_action");
        }

        protected override void DeserializeStart(MovementSwitchBehaviorExecutionParameters parameters)
        {
            parameters.MovementType = (MovementType) parameters.Context.Reader.Read<uint>();
            switch (parameters.MovementType)
            {
                case MovementType.Ground:
                    parameters.BehaviorExecutionParameters = GroundBehavior.DeserializeStart(parameters.Context, 
                        parameters.BranchContext);
                    parameters.ToExecute = GroundBehavior;
                    return;
                case MovementType.Jump:
                    parameters.BehaviorExecutionParameters = JumpBehavior.DeserializeStart(parameters.Context,
                        parameters.BranchContext);
                    parameters.ToExecute = JumpBehavior;
                    return;
                case MovementType.Falling:
                    parameters.BehaviorExecutionParameters = FallingBehavior.DeserializeStart(parameters.Context,
                        parameters.BranchContext);
                    parameters.ToExecute = FallingBehavior;
                    return;
                case MovementType.DoubleJump:
                    parameters.BehaviorExecutionParameters = DoubleJumpBehavior.DeserializeStart(parameters.Context,
                        parameters.BranchContext);
                    parameters.ToExecute = DoubleJumpBehavior;
                    return;
                case MovementType.Jetpack:
                    parameters.BehaviorExecutionParameters = JetpackBehavior.DeserializeStart(parameters.Context,
                        parameters.BranchContext);
                    parameters.ToExecute = JetpackBehavior;
                    return;
                case MovementType.Stunned:
                    return;
                default:
                    throw new Exception($"Invalid {nameof(MovementType)}! Got {parameters.MovementType}!");
            }
        }

        protected override async Task ExecuteStart(MovementSwitchBehaviorExecutionParameters parameters)
        {
            await parameters.ToExecute.ExecuteStart(parameters.BehaviorExecutionParameters);
        }
    }
}