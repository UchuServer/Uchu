using System.Threading.Tasks;
using Uchu.Core;

namespace Uchu.World.Behaviors
{
    public class BasicAttackBehavior : BehaviorBase
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.BasicAttack;
        
        public BehaviorBase OnSuccess { get; set; }
        
        public int MinDamage { get; set; }
        
        public int MaxDamage { get; set; }
        
        public override async Task BuildAsync()
        {
            OnSuccess = await GetBehavior("on_success");

            MinDamage = await GetParameter<int>("min damage");

            MaxDamage = await GetParameter<int>("max damage");
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
            context.Writer.WriteBit(true);

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

        public override async Task CalculateAsync(NpcExecutionContext context, ExecutionBranchContext branchContext)
        {
            Logger.Debug($"NPC is attacking: {branchContext.Target}");
            
            context.Writer.Align();

            context.Writer.Write<ushort>(0);
            
            context.Writer.WriteBit(false);
            context.Writer.WriteBit(false);
            context.Writer.WriteBit(true);
            
            context.Writer.Write(0);

            var success = context.IsValidTarget(branchContext.Target) && context.Alive;

            var damage = (uint) (success ? MinDamage : 0);

            context.Writer.Write(damage);
            
            if (branchContext.Target is Player player)
                player.SendChatMessage($"Attacked for {damage}");

            context.Writer.WriteBit(success);

            if (success)
            {
                var stats = branchContext.Target.GetComponent<Stats>();

                stats.Damage(damage, context.Associate);
                
                await OnSuccess.CalculateAsync(context, branchContext);
            }
        }
    }
}