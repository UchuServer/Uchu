using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace Uchu.World.Behaviors
{
    public class AreaOfEffect : BehaviorBase
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.AreaOfEffect;
        
        public BehaviorBase Action { get; set; }
        
        public int MaxTargets { get; set; }
        
        public float Radius { get; set; }
        
        public override async Task BuildAsync()
        {
            Action = await GetBehavior("action");

            MaxTargets = await GetParameter<int>("max targets");

            Radius = await GetParameter<float>("radius");
        }

        public override async Task ExecuteAsync(ExecutionContext context, ExecutionBranchContext branchContext)
        {
            await base.ExecuteAsync(context, branchContext);

            var length = context.Reader.Read<uint>();
            
            if (length > 10) length = 10; // TODO: Fix

            context.Writer.Write(length);
            
            var targets = new GameObject[length];

            for (var i = 0; i < length; i++)
            {
                var id = context.Reader.Read<ulong>();

                context.Writer.Write(id);

                context.Associate.Zone.TryGetGameObject((long) id, out var target);
                
                targets[i] = target;
            }
            
            foreach (var target in targets)
            {
                await Action.ExecuteAsync(context, new ExecutionBranchContext(target));
            }
        }

        public override async Task CalculateAsync(NpcExecutionContext context, ExecutionBranchContext branchContext)
        {
            if (!context.Associate.TryGetComponent<BaseCombatAiComponent>(out var baseCombatAiComponent)) return;

            var validTarget = baseCombatAiComponent.SeekValidTargets();

            var sourcePosition = context.CalculatingPosition;

            var targets = validTarget.Where(target =>
            {
                var transform = target.Transform;

                var distance = Vector3.Distance(transform.Position, sourcePosition);

                var valid = distance <= Radius;

                return valid;
            }).ToArray();

            foreach (var target in targets)
            {
                if (target is Player player)
                {
                    player.SendChatMessage("You are a AOE target!");
                }
            }
            
            if (targets.Length > 0)
                context.FoundTarget = true;

            context.Writer.Write((uint) targets.Length);

            foreach (var target in targets)
            {
                context.Writer.Write(target);
            }

            foreach (var target in targets)
            {
                await Action.CalculateAsync(context, new ExecutionBranchContext(target));
            }
        }
    }
}