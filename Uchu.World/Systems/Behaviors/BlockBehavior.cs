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
            destroyable.Shielded = true;
            BlocksLeft = BlockableAttacks;
            //Console.WriteLine("Blockable attacks: " + BlocksLeft);
            Action blockAction = default;
            Action finish = default;
            blockAction = (() => 
            {
                BlocksLeft--;
                //Console.WriteLine("Blocks left: " + BlocksLeft);
                if (BlocksLeft <= 0){
                    destroyable.Shielded = false;
                    destroyable.OnAttacked.RemoveListener(blockAction);
                    startNode.End.RemoveListener(finish);
                    //Console.WriteLine("should've removed both listeners and broke shield");
                    BreakAction.ExecuteStart(parameters.Parameters);
                }
            });
            finish = (() => 
            {
                destroyable.Shielded = false;
                BlocksLeft = 0;
                destroyable.OnAttacked.RemoveListener(blockAction);
                startNode.End.RemoveListener(finish);
                //Console.WriteLine("should've removed both listeners");
            });
            destroyable.OnAttacked.AddListener(blockAction);
            startNode.End.AddListener(finish);
        }
    }
}