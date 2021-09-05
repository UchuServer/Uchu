using System;
using System.IO;
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
        public uint Handle { get; set; }

        public BlockBehaviorExecutionParameters(ExecutionContext context, ExecutionBranchContext branchContext) 
            : base(context, branchContext)
        {
        }
    }
    public class BlockBehavior : BehaviorBase<BlockBehaviorExecutionParameters>
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.Block;
        private bool Active { get; set; }

        private bool BlockDamage { get; set; }

        private bool BlockKnockback { get; set; }

        private bool BlockStun { get; set; }

        private BehaviorBase BreakAction { get; set; }

        private int BlockableAttacks { get; set; }

        private int BlocksLeft { get; set; }

        private bool Broken { get; set; }

        public override async Task BuildAsync()
        {
            BlockDamage = await GetParameter<int>("block_damage") == 1;
            BlockKnockback = await GetParameter<int>("block_knockback") == 1;
            BlockStun = await GetParameter<int>("block_stuns") == 1;
            BreakAction = await GetBehavior("break_action");
            BlockableAttacks = await GetParameter<int>("num_attacks_can_block");
        }

        protected override void DeserializeStart(BitReader reader, BlockBehaviorExecutionParameters parameters)
        {
            //parameters.Parameters = BreakAction.DeserializeStart(reader, parameters.Context, parameters.BranchContext);
            if (!(parameters.Context.Associate.TryGetComponent<DestroyableComponent>(out var destroyable)) || Active) return;
            if (parameters.BranchContext.StartNode == default) return; //prevent accidental invincibility
            Active = true;
            Broken = false;
            var startNode = parameters.BranchContext.StartNode;
            destroyable.Shielded = BlockDamage;
            destroyable.ShieldedKnockback = BlockKnockback;
            destroyable.ShieldedStun = BlockStun;
            BlocksLeft = BlockableAttacks;
            Action blockAction = default;
            Action finish = default;
            Action end = (() => 
            {
                destroyable.Shielded = false;
                destroyable.ShieldedKnockback = false;
                destroyable.ShieldedStun = false;
                Active = false;
                BlocksLeft = 0;
                destroyable.OnAttacked.RemoveListener(blockAction);
                startNode.End.RemoveListener(finish);
            });
            blockAction = (() => 
            {
                if (!Active)
                {
                    destroyable.OnAttacked.RemoveListener(blockAction);
                    startNode.End.RemoveListener(finish);
                    return;
                }
                BlocksLeft--;
                Console.WriteLine(BlocksLeft);
                if (BlocksLeft <= 0){
                    end();
                    Broken = true;
                }
            });
            finish = (() => 
            {
                if (!Active)
                {
                    destroyable.OnAttacked.RemoveListener(blockAction);
                    startNode.End.RemoveListener(finish);
                    return;
                }
                end();
            });
            
            destroyable.OnAttacked.AddListener(blockAction);
            startNode.End.AddListener(finish);
        }
        protected override void SerializeStart(BitWriter writer, BlockBehaviorExecutionParameters parameters)
        {
            //parameters.Parameters = BreakAction.SerializeStart(writer, parameters.NpcContext, parameters.BranchContext);
        }
        protected override void ExecuteSync(BlockBehaviorExecutionParameters parameters)
        {
            Active = false;
            if (Broken) BreakAction.ExecuteStart(parameters.Parameters);
        }
        protected override void SerializeSync(BitWriter writer, BlockBehaviorExecutionParameters parameters)
        {
            if (!Broken) return;
            parameters.Parameters = BreakAction.SerializeStart(writer, parameters.NpcContext,
                parameters.BranchContext);
        }
    }
}