using System.Threading.Tasks;
using RakDotNet.IO;

namespace Uchu.World.Systems.Behaviors
{
    public class StunBehavior : BehaviorBase
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.Stun;
        
        private int CantAttack { get; set; }

        private int CantEquip { get; set; }
        
        private int CantInteract { get; set; }

        private int CantJump { get; set; }

        private int CantMove { get; set; }

        private int CantTurn { get; set; }

        private int CantUseItem { get; set; }

        private int StunCaster { get; set; }
        
        public override async Task BuildAsync()
        {
            CantAttack = await GetParameter<int>("cant_attack");
            CantEquip = await GetParameter<int>("cant_equip");
            CantInteract = await GetParameter<int>("cant_interact");
            CantJump = await GetParameter<int>("cant_jump");
            CantMove = await GetParameter<int>("cant_move");
            CantTurn = await GetParameter<int>("cant_turn");
            CantUseItem = await GetParameter<int>("cant_use_item"); //none of these do anything yet but i'm putting them here for future use
            StunCaster = await GetParameter<int>("stun_caster"); //i also think all of these are bools
        }

        public override BehaviorExecutionParameters DeserializeStart(BitReader reader, ExecutionContext context,
            ExecutionBranchContext branchContext)
        {
            if (StunCaster != 1 && branchContext.Target != context.Associate)
                reader.ReadBit();
            return base.DeserializeStart(reader, context, branchContext);
        }

        public override BehaviorExecutionParameters SerializeStart(BitWriter writer, NpcExecutionContext context,
            ExecutionBranchContext branchContext)
        {
            
            if (StunCaster != 1)
                if (branchContext.Target.TryGetComponent<DestroyableComponent>(out var destroyable)) writer.WriteBit(destroyable.ShieldedStun);
                else writer.WriteBit(false);
            return base.SerializeStart(writer, context, branchContext);
        }
        public override void ExecuteStart(BehaviorExecutionParameters parameters)
        {
            System.Console.WriteLine(StunCaster == 0);
            System.Console.WriteLine(parameters.BranchContext.Target != null);
            System.Console.WriteLine(parameters.BranchContext.Duration != 0);
            if (parameters.BranchContext.Target != null){
                System.Console.WriteLine(parameters.BranchContext.Target.TryGetComponent<BaseCombatAiComponent>(out var no));
            } else {
                System.Console.WriteLine("no basecombatai");
            }
            if (StunCaster == 0 && parameters.BranchContext.Target != null && parameters.BranchContext.Duration != 0 &&
                parameters.BranchContext.Target.TryGetComponent<BaseCombatAiComponent>(out var ai))
            {
                System.Console.WriteLine("stunned");
                ai.Stunned = true;
                parameters.Schedule(() => {
                    ai.Stunned = false;
                }, parameters.BranchContext.Duration);
            }
        }
    }
}