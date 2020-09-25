using System;
using System.Threading.Tasks;
using Uchu.Core;

namespace Uchu.World.Systems.Behaviors
{
    public class BasicAttackBehavior : BehaviorBase
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.BasicAttack;
        
        /// <summary>
        /// The behavior to execute if this behavior succeeded
        /// </summary>
        private BehaviorBase OnSuccess { get; set; }
        
        /// <summary>
        /// The minimum damage this attack may do
        /// </summary>
        private uint MinDamage { get; set; }
        
        /// <summary>
        /// The maximum damage this attack may do
        /// </summary>
        private uint MaxDamage { get; set; }
        
        /// <summary>
        /// Fills the success, min and max damage parameters
        /// </summary>
        /// <returns></returns>
        public override async Task BuildAsync()
        {
            OnSuccess = await GetBehavior("on_success");
            MinDamage = await GetParameter<uint>("min damage");
            MaxDamage = await GetParameter<uint>("max damage");
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
            var _ = context.Reader.ReadBit(); // Died?
            var successSate = context.Reader.Read<byte>();

            // Make sure the target is valid and damage them
            if (branchContext.Target != default && branchContext.Target.TryGetComponent<Stats>(out var stats))
            {
                stats.Damage(CalculateDamage(damage), context.Associate);
            }
            
            // Execute the success state only if some parameters are set
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
            
            // Three unknowns
            context.Writer.Write<byte>(0);
            context.Writer.Write<byte>(0);
            context.Writer.Write<byte>(0);
            
            context.Writer.WriteBit(false);
            context.Writer.WriteBit(false);
            context.Writer.WriteBit(true);
            
            // Unknown 2 == true so this should be set
            context.Writer.Write<uint>(0);

            var damage = (uint)new Random().Next((int)MinDamage, (int)MaxDamage);
            context.Writer.Write(damage);
            
            context.Writer.WriteBit(!context.Alive);
            
            var success = context.IsValidTarget(branchContext.Target) && context.Alive;
            context.Writer.Write<uint>((uint)(success ? 1 : 0));

            if (success && branchContext.Target.TryGetComponent<Stats>(out var stats))
            {
                await PlayFxAsync("onhit", branchContext.Target, 1000);
                stats.Damage(damage, context.Associate, EffectHandler);
                await OnSuccess.CalculateAsync(context, branchContext);
            }
        }
        
        /// <summary>
        /// Takes a damage input and ensures that it's between the min and max damage
        /// </summary>
        /// <param name="damage">The damage to check</param>
        /// <returns>the damage, or the min or max damage</returns>
        private uint CalculateDamage(uint damage)
        {
            if (damage <= MaxDamage && damage >= MinDamage) return damage;
            return damage > MaxDamage ? MaxDamage : MinDamage;
        }
    }
}