using System;
using System.Threading.Tasks;
using Uchu.Core;

namespace Uchu.World.Behaviors
{
    public class BasicAttackBehavior : BehaviorBase
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.BasicAttack;
        
        public override Task BuildAsync()
        {
            return Task.CompletedTask;
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

            foreach (var target in branchContext.Targets)
            {
                target.GetComponent<Stats>().Damage(damage, context.Associate);
            }
        }
    }
}