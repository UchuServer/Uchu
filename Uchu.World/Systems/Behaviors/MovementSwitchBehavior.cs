using System;
using System.Threading.Tasks;
using Uchu.Core;

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
        private BehaviorBase AirBehavior { get; set; }
        private BehaviorBase JetpackBehavior { get; set; }
        private BehaviorBase MovingBehavior { get; set; }

        public override async Task BuildAsync()
        {
            GroundBehavior = await GetBehavior("ground_action");
            JumpBehavior = await GetBehavior("jump_action");
            FallingBehavior = await GetBehavior("falling_action");
            DoubleJumpBehavior = await GetBehavior("double_jump_action");
            AirBehavior = await GetBehavior("air_action");
            JetpackBehavior = await GetBehavior("ground_action");
            MovingBehavior = await GetBehavior("moving_action");
        }

        protected override void DeserializeStart(MovementSwitchBehaviorExecutionParameters parameters)
        {
            parameters.MovementType = (MovementType) parameters.Context.Reader.Read<uint>();
            switch (parameters.MovementType)
            {
                case MovementType.Moving:
                    Logger.Debug("Received movement switch type moving, guessing ground behavior");
                    parameters.ToExecute = GroundBehavior;
                    break;
                case MovementType.Ground:
                    Logger.Debug("Ground");
                    parameters.ToExecute = GroundBehavior;
                    break;
                case MovementType.Jump:
                    Logger.Debug("Jump");
                    parameters.ToExecute = JumpBehavior;
                    break;
                case MovementType.Falling:
                    Logger.Debug("Falling");
                    parameters.ToExecute = FallingBehavior;
                    break;
                case MovementType.DoubleJump:
                    Logger.Debug("DoubleJump");
                    parameters.ToExecute = DoubleJumpBehavior;
                    break;
                case MovementType.Air:
                    Logger.Debug("Air");
                    parameters.ToExecute = AirBehavior;
                    break;
                case MovementType.Jetpack:
                    Logger.Debug("JetPack");
                    parameters.ToExecute = JetpackBehavior;
                    break;
                case MovementType.Unknown:
                    Logger.Debug("Received movement switch type unknown, guessing ground behavior");
                    parameters.ToExecute = GroundBehavior;
                    break;
                default:
                    throw new Exception($"Invalid {nameof(MovementType)}! Got {parameters.MovementType}!");
            }
            
            parameters.BehaviorExecutionParameters = parameters.ToExecute.DeserializeStart(parameters.Context, 
                parameters.BranchContext);
        }

        protected override async Task ExecuteStart(MovementSwitchBehaviorExecutionParameters parameters)
        {
            if (parameters.ToExecute == null)
                return;    
            await parameters.ToExecute.ExecuteStart(parameters.BehaviorExecutionParameters);
        }
    }
}