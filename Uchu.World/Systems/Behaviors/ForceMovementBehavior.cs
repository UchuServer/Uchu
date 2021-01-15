using System.Linq;
using System.Threading.Tasks;
using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.World.Systems.Behaviors
{
    public class ForceMovementBehaviorExecutionParameters : BehaviorExecutionParameters
    {
        public uint Handle { get; set; }
        public BehaviorBase Action { get; set; }
        public BehaviorExecutionParameters ActionExecutionParameters { get; set; }

        public ForceMovementBehaviorExecutionParameters(ExecutionContext context, ExecutionBranchContext branchContext) 
            : base(context, branchContext)
        {
        }
    }
    public class ForceMovementBehavior : BehaviorBase<ForceMovementBehaviorExecutionParameters>
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.ForceMovement;

        private BehaviorBase HitAction { get; set; }

        private BehaviorBase HitActionEnemy { get; set; }

        private BehaviorBase HitActionFaction { get; set; }
        
        public override void Build()
        {
            HitAction = GetBehavior("hit_action");
            HitActionEnemy = GetBehavior("hit_action_enemy");
            HitActionFaction = GetBehavior("hit_action_faction");
        }

        protected override void DeserializeStart(BitReader reader, ForceMovementBehaviorExecutionParameters parameters)
        {
            if (HitAction.BehaviorId == 0 && HitActionEnemy.BehaviorId == 0 && HitActionFaction.BehaviorId == 0)
                return;
            
            parameters.Handle = reader.Read<uint>();
            parameters.RegisterHandle<ForceMovementBehaviorExecutionParameters>(parameters.Handle, DeserializeSync, 
                ExecuteSync);
        }

        protected override void DeserializeSync(BitReader reader, ForceMovementBehaviorExecutionParameters parameters)
        {
            var actionId = reader.Read<uint>();
            parameters.Action = GetBehavior(actionId);
            
            var targetId = reader.Read<ulong>();
            parameters.Context.Associate.Zone.TryGetGameObject((long) targetId, out var target);
            
            parameters.ActionExecutionParameters = parameters.Action.DeserializeStart(reader,
                parameters.Context, new ExecutionBranchContext
                {
                    Duration = parameters.BranchContext.Duration,
                    Target = target
                });
        }

        protected override void ExecuteSync(ForceMovementBehaviorExecutionParameters parameters)
        {
            parameters.Action.ExecuteStart(parameters.ActionExecutionParameters);
        }
    }
}