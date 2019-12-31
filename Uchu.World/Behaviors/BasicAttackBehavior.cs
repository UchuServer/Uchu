using System;
using System.Threading.Tasks;
using Uchu.Core;

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

            context.Reader.Read<ushort>();

            context.Reader.ReadBit();
            context.Reader.ReadBit();
            context.Reader.ReadBit();

            context.Reader.Read<uint>();

            var damage = context.Reader.Read<uint>();

            ((Player) context.Associate)?.SendChatMessage($"{damage} -> {branchContext.Target}");
            
            branchContext.Target.GetComponent<Stats>().Damage(damage, context.Associate);

            if (context.Reader.ReadBit())
            {
                await OnSuccess.ExecuteAsync(context, branchContext);
            }
        }
    }
}