using System.Linq;
using System.Threading.Tasks;
using Uchu.Core;

namespace Uchu.World.Systems.Behaviors
{
    public class ForceMovementBehaviorExecutionParameters : BehaviorExecutionParameters
    {
        public uint Handle { get; set; }
        public BehaviorBase Action { get; set; }
        public BehaviorExecutionParameters ActionExecutionParameters { get; set; }
    }
    public class ForceMovementBehavior : BehaviorBase<ForceMovementBehaviorExecutionParameters>
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.ForceMovement;

        private BehaviorBase HitAction { get; set; }

        private BehaviorBase HitActionEnemy { get; set; }

        private BehaviorBase HitActionFaction { get; set; }
        
        public override async Task BuildAsync()
        {
            HitAction = await GetBehavior("hit_action");
            HitActionEnemy = await GetBehavior("hit_action_enemy");
            HitActionFaction = await GetBehavior("hit_action_faction");
        }

        protected override void DeserializeStart(ForceMovementBehaviorExecutionParameters behaviorExecutionParameters)
        {
            if (HitAction.BehaviorId == 0 && HitActionEnemy.BehaviorId == 0 && HitActionFaction.BehaviorId == 0) return;
            behaviorExecutionParameters.Handle = behaviorExecutionParameters.Context.Reader.Read<uint>();
            RegisterHandle(behaviorExecutionParameters.Handle, behaviorExecutionParameters);
        }

        protected override async void DeserializeSync(ForceMovementBehaviorExecutionParameters behaviorExecutionParameters)
        {
            var actionId = behaviorExecutionParameters.Context.Reader.Read<uint>();
            behaviorExecutionParameters.Action = await GetBehavior(actionId);
            
            behaviorExecutionParameters.ActionExecutionParameters = behaviorExecutionParameters.Action.DeserializeStart(
                behaviorExecutionParameters.Context, behaviorExecutionParameters.BranchContext);
            
            var targetId = behaviorExecutionParameters.Context.Reader.Read<ulong>();
            behaviorExecutionParameters.Context.Associate.Zone.TryGetGameObject((long) targetId, out var target);
            behaviorExecutionParameters.ActionExecutionParameters.BranchContext.Target = target;
        }

        protected override async Task ExecuteSync(ForceMovementBehaviorExecutionParameters behaviorExecutionParameters)
        {
            await behaviorExecutionParameters.Action.ExecuteStart(behaviorExecutionParameters
                .ActionExecutionParameters);
        }
    }
}