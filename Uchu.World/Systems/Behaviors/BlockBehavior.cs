using System;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Uchu.Physics;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.World.Systems.Behaviors
{
    public class BlockBehavior : BehaviorBase
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

        public override void ExecuteStart(BehaviorExecutionParameters parameters)
        {
            if (!(parameters.Context.Associate.TryGetComponent<DestroyableComponent>(out var destroyable))) return;
            Console.WriteLine("got the destroyable");
            if (parameters.BranchContext.StartNode == default) return; //prevent accidental invincibility
            Console.WriteLine("got past branch check");
            var startNode = parameters.BranchContext.StartNode;
            destroyable.Shielded = true;
            BlocksLeft = BlockableAttacks;
            Action blockAction = default;
            Action finish = default;
            blockAction = (() => 
            {
                Console.WriteLine("should block attack");
                BlocksLeft--;
                if (BlocksLeft == 0){
                    destroyable.Shielded = false;
                    BreakAction.ExecuteStart(parameters);
                    destroyable.OnAttacked.RemoveListener(blockAction);
                    startNode.End.RemoveListener(finish);
                }
            });
            finish = (() => 
            {
                destroyable.Shielded = false;
                BlocksLeft = 0;
                destroyable.OnAttacked.RemoveListener(blockAction);
                startNode.End.RemoveListener(finish);
            });
            destroyable.OnAttacked.AddListener(blockAction);
            startNode.End.AddListener(finish);
        }
    }
}