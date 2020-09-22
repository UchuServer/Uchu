using System;
using System.Threading.Tasks;
using Uchu.Core;

namespace Uchu.World.Systems.Behaviors
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

            var unknown = context.Reader.Read<byte>();
            var unknown1 = unknown > 0 ? unknown : context.Reader.Read<byte>();
            context.Reader.Read<byte>();

            context.Reader.ReadBit();
            context.Reader.ReadBit();
            var flag2 = context.Reader.ReadBit();

            if (flag2) context.Reader.Read<uint>();
            
            var damage = context.Reader.Read<uint>();
            var died = context.Reader.ReadBit();
            var successSate = context.Reader.Read<byte>();

            if (branchContext.Target == default || !branchContext.Target.TryGetComponent<Stats>(out var stats))
            {
                Logger.Error($"Invalid target: {branchContext.Target}");
            }
            else
            {
                await Task.Run(() =>
                {
                    stats.Damage(damage, context.Associate);
                });
            }
            
            if (unknown1 == 81) context.Reader.Read<byte>();
            if (successSate == 1)
            {
                await OnSuccess.ExecuteAsync(context, branchContext);
            }
        }

        public override async Task CalculateAsync(NpcExecutionContext context, ExecutionBranchContext branchContext)
        {
            context.Associate.Transform.LookAt(branchContext.Target.Transform.Position);
            
            await branchContext.Target.NetFavorAsync();

            context.Writer.Align();

            context.Writer.Write<ushort>(0);
            
            context.Writer.WriteBit(false);
            context.Writer.WriteBit(false);
            context.Writer.WriteBit(true);
            
            context.Writer.Write(0);

            var success = context.IsValidTarget(branchContext.Target) && context.Alive;

            var damage = (uint) (success ? MinDamage : 0);

            context.Writer.Write(damage);
            
            context.Writer.WriteBit(success);

            if (success)
            {
                await PlayFxAsync("onhit", branchContext.Target, 1000);

                var stats = branchContext.Target.GetComponent<Stats>();

                stats.Damage(damage, context.Associate, EffectHandler);
                
                await OnSuccess.CalculateAsync(context, branchContext);
            }
        }
    }
}