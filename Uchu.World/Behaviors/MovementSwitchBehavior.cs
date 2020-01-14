using System.Threading.Tasks;

namespace Uchu.World.Behaviors
{
    public class MovementSwitchBehavior : BehaviorBase
    {
        public MovementType MovementType { get; private set; }

        public override BehaviorTemplateId Id => BehaviorTemplateId.MovementSwitch;
        
        public BehaviorBase GroundBehavior { get; set; }
        
        public BehaviorBase JumpBehavior { get; set; }
        
        public BehaviorBase FallingBehavior { get; set; }
        
        public BehaviorBase DoubleJumpBehavior { get; set; }
        
        public BehaviorBase JetpackBehavior { get; set; }

        public override async Task BuildAsync()
        {
            GroundBehavior = await GetBehavior("ground_action");
            JumpBehavior = await GetBehavior("jump_action");
            FallingBehavior = await GetBehavior("falling_action");
            DoubleJumpBehavior = await GetBehavior("double_jump_action");
            JetpackBehavior = await GetBehavior("ground_action");
        }

        public override async Task ExecuteAsync(ExecutionContext context, ExecutionBranchContext branchContext)
        {
            await base.ExecuteAsync(context, branchContext);
            
            MovementType = (MovementType) context.Reader.Read<uint>();

            context.Writer.Write((uint) MovementType);

            switch (MovementType)
            {
                case MovementType.Ground:
                    await GroundBehavior.ExecuteAsync(context, branchContext);
                    return;
                case MovementType.Jump:
                    await JumpBehavior.ExecuteAsync(context, branchContext);
                    return;
                case MovementType.Falling:
                    await FallingBehavior.ExecuteAsync(context, branchContext);
                    return;
                case MovementType.DoubleJump:
                    await DoubleJumpBehavior.ExecuteAsync(context, branchContext);
                    return;
                case MovementType.Jetpack:
                    await JetpackBehavior.ExecuteAsync(context, branchContext);
                    return;
                default:
                    return;
            }
        }
    }
}