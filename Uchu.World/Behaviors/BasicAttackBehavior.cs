using System.Threading.Tasks;

namespace Uchu.World.Behaviors
{
    public class BasicAttackBehavior : BehaviorBase
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.BasicAttack;
        
        public BehaviorBase OnSuccess { get; set; }
        
        public override async Task BuildAsync()
        {
            OnSuccess = await GetBehavior("on_success");
        }

        public override async Task ExecuteAsync(ExecutionContext context, ExecutionBranchContext branchContext)
        {
            await base.ExecuteAsync(context, branchContext);
            
            context.Reader.Align();
            context.Writer.Align();

            context.Reader.Read<ushort>();
            context.Writer.Write<ushort>(0);

            context.Reader.ReadBit();
            context.Reader.ReadBit();
            context.Reader.ReadBit();
            context.Writer.WriteBit(false);
            context.Writer.WriteBit(false);
            context.Writer.WriteBit(false);

            context.Reader.Read<uint>();
            context.Writer.Write(0);

            var damage = context.Reader.Read<uint>();
            context.Writer.Write(damage);

            ((Player) context.Associate)?.SendChatMessage($"{damage} -> {branchContext.Target}");
            
            branchContext.Target.GetComponent<Stats>().Damage(damage, context.Associate);

            var success = context.Reader.ReadBit();
            context.Writer.WriteBit(success);
            
            if (success)
            {
                await OnSuccess.ExecuteAsync(context, branchContext);
            }
        }
    }
}