using System;
using System.Threading.Tasks;
using Uchu.Core;

namespace Uchu.World.Systems.Behaviors
{
    public class BasicAttackBehaviorExecutionParameters : BehaviorExecutionParameters
    {
        public bool ServerSide { get; set; }
        public byte Unknown { get; set; }
        public byte Unknown1 { get; set; }
        public bool Flag2 { get; set; }
        public uint Damage { get; set; }
        public byte SuccessState { get; set; }
        public BehaviorExecutionParameters OnSuccessBehaviorExecutionParameters { get; set; }
        
        public BasicAttackBehaviorExecutionParameters(ExecutionContext context, ExecutionBranchContext branchContext) 
            : base(context, branchContext)
        {
            ServerSide = false;
        }
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

            // Unknown 2
            behaviorExecutionParameters.Context.Reader.Read<byte>();
            
            // Unused flags
            behaviorExecutionParameters.Context.Reader.ReadBit();
            behaviorExecutionParameters.Context.Reader.ReadBit();
            
            behaviorExecutionParameters.Flag2 = behaviorExecutionParameters.Context.Reader.ReadBit();
            if (behaviorExecutionParameters.Flag2)
                behaviorExecutionParameters.Context.Reader.Read<uint>();

            behaviorExecutionParameters.Damage = behaviorExecutionParameters.Context.Reader.Read<uint>();
            behaviorExecutionParameters.Context.Reader.ReadBit(); // Died?
            behaviorExecutionParameters.SuccessState = behaviorExecutionParameters.Context.Reader.Read<byte>();
            
            if (behaviorExecutionParameters.Unknown1 == 81)
                behaviorExecutionParameters.Context.Reader.Read<byte>();
            
            if (behaviorExecutionParameters.SuccessState == 1)
                behaviorExecutionParameters.OnSuccessBehaviorExecutionParameters = OnSuccess.DeserializeStart(
                    behaviorExecutionParameters.Context, behaviorExecutionParameters.BranchContext);
        }

        protected override void ExecuteStart(BasicAttackBehaviorExecutionParameters parameters)
        {
            // Store as function as server side and client side execution is scheduled differently
            parameters.NetFavor(() =>
            {
                if (parameters.BranchContext.Target != default &&
                    parameters.BranchContext.Target.TryGetComponent<DestroyableComponent>(out var stats) &&
                    (parameters.ServerSide && parameters.SuccessState == 1 || !parameters.ServerSide))
                {
                    if (parameters.ServerSide)
                        parameters.PlayFX("onhit", EffectId);

                    // This is ran as a background task as it may trigger many async messages
                    Task.Factory.StartNew(
                        () => stats.Damage(CalculateDamage(parameters.Damage), parameters.Context.Associate),
                        TaskCreationOptions.LongRunning);
                }

                // Execute the success state only if some parameters are set
                if (parameters.SuccessState == 1)
                {
                    OnSuccess.ExecuteStart(parameters.OnSuccessBehaviorExecutionParameters);
                }
            });
        }

        protected override void SerializeStart(BasicAttackBehaviorExecutionParameters parameters)
        {
            parameters.ServerSide = true;
            parameters.NpcContext.Associate.Transform.LookAt(parameters.BranchContext.Target.Transform.Position);
            parameters.NpcContext.Writer.Align();
            
            // Three unknowns
            parameters.NpcContext.Writer.Write<byte>(0);
            parameters.NpcContext.Writer.Write<byte>(0);
            parameters.NpcContext.Writer.Write<byte>(0);
            
            parameters.NpcContext.Writer.WriteBit(false);
            parameters.NpcContext.Writer.WriteBit(false);
            parameters.NpcContext.Writer.WriteBit(true);
            
            // flag 2 == true so this should be set
            parameters.NpcContext.Writer.Write<uint>(0);

            var damage = (uint)new Random().Next((int)MinDamage, (int)MaxDamage);
            parameters.NpcContext.Writer.Write(damage);
            
            parameters.NpcContext.Writer.WriteBit(!parameters.NpcContext.Alive);
            
            var success = parameters.NpcContext.IsValidTarget(parameters.BranchContext.Target) && 
                          parameters.NpcContext.Alive;
            parameters.SuccessState = (byte) (success ? 1 : 0);
            parameters.NpcContext.Writer.Write(parameters.SuccessState);

            if (success)
            {
                parameters.OnSuccessBehaviorExecutionParameters = OnSuccess.SerializeStart(parameters.NpcContext,
                    parameters.BranchContext);
            }
        }
        
        /// <summary>
        /// Takes a damage input and ensures that it's between the min and max damage
        /// </summary>
        /// <param name="damage">The damage to check</param>
        /// <returns>the damage, or the min or max damage</returns>
        private uint CalculateDamage(uint damage)
        {
            if (MinDamage == 0 && MaxDamage == 0 ||damage >= MinDamage && damage <= MaxDamage)
                return damage;
            return damage > MaxDamage ? MaxDamage : MinDamage;
        }
    }
}