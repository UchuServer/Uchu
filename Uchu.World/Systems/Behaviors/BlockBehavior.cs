using System;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using RakDotNet.IO;
using Uchu.Physics;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.World.Systems.Behaviors
{
    public class BlockBehaviorExecutionParameters : BehaviorExecutionParameters
    {
        public BehaviorExecutionParameters Parameters { get; set; }

        public BlockBehaviorExecutionParameters(ExecutionContext context, ExecutionBranchContext branchContext) 
            : base(context, branchContext)
        {
        }
    }
    public class BlockBehavior : BehaviorBase<BlockBehaviorExecutionParameters>
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.Block;

        private bool BlockDamage { get; set; }

        private bool BlockKnockback { get; set; }

        private bool BlockStun { get; set; }

        private BehaviorBase BreakAction { get; set; }

        private int BlockableAttacks { get; set; }

        private int BlocksLeft { get; set; }

        public override async Task BuildAsync()
        {
            BlockDamage = await GetParameter<bool>("block_damage");
            BlockKnockback = await GetParameter<bool>("block_knockback");
            BlockStun = await GetParameter<bool>("block_stuns");
            BreakAction = await GetBehavior("break_action");
            BlockableAttacks = await GetParameter<int>("num_attacks_can_block");
        }

        protected override void DeserializeStart(BitReader reader, BlockBehaviorExecutionParameters parameters)
        {
            parameters.Parameters = BreakAction.DeserializeStart(reader, parameters.Context, parameters.BranchContext);
            //how do these work? this behavior is so different compared to the others, i have a feeling this wouldn't
            //work in some circumstances. you could unblock, but how would the game predict that beforehand?
        }
        protected override void SerializeStart(BitWriter writer, BlockBehaviorExecutionParameters parameters)
        {
            parameters.Parameters = BreakAction.SerializeStart(writer, parameters.NpcContext, parameters.BranchContext);
        }
        protected override void ExecuteStart(BlockBehaviorExecutionParameters parameters)
        {
            if (!(parameters.Context.Associate.TryGetComponent<DestroyableComponent>(out var destroyable))) return;
            if (parameters.BranchContext.StartNode == default) return; //prevent accidental invincibility
            var startNode = parameters.BranchContext.StartNode;
            destroyable.Shielded = BlockDamage;
            destroyable.ShieldedKnockback = BlockKnockback;
            destroyable.ShieldedStun = BlockStun;
            BlocksLeft = BlockableAttacks;
            Action blockAction = default;
            Action finish = default;
            blockAction = (() => 
            {
                BlocksLeft--;
                if (BlocksLeft <= 0){
                    destroyable.Shielded = false;
                    destroyable.ShieldedKnockback = false;
                    destroyable.ShieldedStun = false;
                    destroyable.OnAttacked.RemoveListener(blockAction);
                    startNode.End.RemoveListener(finish);
                    BreakAction.ExecuteStart(parameters.Parameters); //this causes an error with reading a bit in knockback, likely because it's being executed so far in the future
                    //after the packets have been sent
                }
            });
            finish = (() => 
            {
                destroyable.Shielded = false;
                destroyable.ShieldedKnockback = false;
                destroyable.ShieldedStun = false;
                BlocksLeft = 0;
                destroyable.OnAttacked.RemoveListener(blockAction);
                startNode.End.RemoveListener(finish);
                //unblocking normally is completely fine from my testing
            });
            destroyable.OnAttacked.AddListener(blockAction);
            startNode.End.AddListener(finish);
        }
    }
}