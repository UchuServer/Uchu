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
            //this actually works
            //wow my brain hurts
            if (!(parameters.Context.Associate.TryGetComponent<DestroyableComponent>(out var destroyable))) return;
            if (parameters.EnclosedContext == default) return; //prevent accidental invincibility
            var blocker = parameters.Context.Associate;
            destroyable.Shielded = true;
            BlocksLeft = BlockableAttacks;
            Action blockAction = default;
            Action finish = default;
            blockAction = (() => 
            {
                BlocksLeft--;
                if (BlocksLeft == 0){
                    destroyable.Shielded = false;
                    BreakAction.ExecuteStart(parameters);
                    destroyable.OnAttacked.RemoveListener(blockAction);
                    parameters.EnclosedContext.End.RemoveListener(finish);
                }
            });
            finish = (() => 
            {
                destroyable.Shielded = false;
                BlocksLeft = 0;
                destroyable.OnAttacked.RemoveListener(blockAction);
                parameters.EnclosedContext.End.RemoveListener(finish);
            });
            destroyable.OnAttacked.AddListener(blockAction);
            parameters.EnclosedContext.End.AddListener(finish);
        }
    }
}