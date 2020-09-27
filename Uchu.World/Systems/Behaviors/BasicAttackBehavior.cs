using System;
using System.Threading.Tasks;
using Uchu.Core;

namespace Uchu.World.Systems.Behaviors
{
    public class BasicAttackBehaviorExecutionParameters : BehaviorExecutionParameters
    {
        public byte Unknown { get; set; }
        public byte Unknown1 { get; set; }
        public bool Flag2 { get; set; }
        public uint Damage { get; set; }
        public byte SuccessState { get; set; }
        public BehaviorExecutionParameters OnSuccessBehaviorExecutionParameters { get; set; }
    }
    public class BasicAttackBehavior : BehaviorBase<BasicAttackBehaviorExecutionParameters>
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

        protected override void DeserializeStart(BasicAttackBehaviorExecutionParameters behaviorExecutionParameters)
        {
            behaviorExecutionParameters.Context.Reader.Align();
            behaviorExecutionParameters.Unknown = behaviorExecutionParameters.Context.Reader.Read<byte>();
            behaviorExecutionParameters.Unknown1 = behaviorExecutionParameters.Unknown > 0
                ? behaviorExecutionParameters.Unknown
                : behaviorExecutionParameters.Context.Reader.Read<byte>();
            
            // Unused flags
            behaviorExecutionParameters.Context.Reader.ReadBit();
            behaviorExecutionParameters.Context.Reader.ReadBit();
            
            behaviorExecutionParameters.Flag2 = behaviorExecutionParameters.Context.Reader.ReadBit();
            if (behaviorExecutionParameters.Flag2) behaviorExecutionParameters.Context.Reader.Read<uint>();

            behaviorExecutionParameters.Damage = behaviorExecutionParameters.Context.Reader.Read<uint>();
            behaviorExecutionParameters.Context.Reader.ReadBit(); // Died?
            behaviorExecutionParameters.SuccessState = behaviorExecutionParameters.Context.Reader.Read<byte>();
            
            if (behaviorExecutionParameters.Unknown1 == 81) behaviorExecutionParameters.Context.Reader.Read<byte>();
            if (behaviorExecutionParameters.SuccessState == 1)
                behaviorExecutionParameters.OnSuccessBehaviorExecutionParameters = OnSuccess.DeserializeStart(
                    behaviorExecutionParameters.Context, behaviorExecutionParameters.BranchContext);
        }

        protected override async Task ExecuteStart(BasicAttackBehaviorExecutionParameters behaviorExecutionParameters)
        {
            // Make sure the target is valid and damage them
            if (behaviorExecutionParameters.BranchContext.Target != default && 
                behaviorExecutionParameters.BranchContext.Target.TryGetComponent<Stats>(out var stats))
            {
                stats.Damage(CalculateDamage(behaviorExecutionParameters.Damage), 
                    behaviorExecutionParameters.Context.Associate);
            }
            
            // Execute the success state only if some parameters are set
            if (behaviorExecutionParameters.SuccessState == 1)
            {
                await OnSuccess.ExecuteStart(behaviorExecutionParameters.OnSuccessBehaviorExecutionParameters);
            }
        }

        public override async Task SerializeStart(NpcExecutionContext context, ExecutionBranchContext branchContext)
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
                await OnSuccess.SerializeStart(context, branchContext);
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