using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.World.Systems.Behaviors
{
    public class AreaOfEffectExecutionParameters : BehaviorExecutionParameters
    {
        public uint Length { get; set; }
        public List<BehaviorExecutionParameters> TargetActions { get; }
        
        public AreaOfEffectExecutionParameters(ExecutionContext context, ExecutionBranchContext branchContext) 
            : base(context, branchContext)
        {
            TargetActions = new List<BehaviorExecutionParameters>();
        }
    }
    
    public class AreaOfEffect : BehaviorBase<AreaOfEffectExecutionParameters>
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.AreaOfEffect;
        private BehaviorBase Action { get; set; }
        private int MaxTargets { get; set; }
        private float Radius { get; set; }
        public override async Task BuildAsync()
        {
            Action = await GetBehavior("action");
            MaxTargets = await GetParameter<int>("max targets");
            Radius = await GetParameter<float>("radius");
        }

        protected override void DeserializeStart(BitReader reader, AreaOfEffectExecutionParameters parameters)
        {
            parameters.Length = reader.Read<uint>();

            var targets = new List<GameObject>();
            for (var i = 0; i < parameters.Length; i++)
            {
                var targetId = reader.Read<long>();
                if (!parameters.Context.Associate.Zone.TryGetGameObject(targetId, 
                    out var target))
                {
                    Logger.Error(
                        $"{parameters.Context.Associate} sent invalid AreaOfEffect target.");
                    continue;
                }
                targets.Add(target);
            }

            foreach (var target in targets)
            {
                var behaviorBase = Action.DeserializeStart(reader, parameters.Context, 
                    new ExecutionBranchContext()
                    {
                        Target = target,
                        Duration = parameters.BranchContext.Duration
                    });
                
                parameters.TargetActions.Add(behaviorBase);
            }
        }

        protected override void ExecuteStart(AreaOfEffectExecutionParameters behaviorExecutionsParameters)
        {
            foreach (var behaviorExecutionParameters in behaviorExecutionsParameters.TargetActions)
            {
                Action.ExecuteStart(behaviorExecutionParameters);
            }
        }

        protected override void SerializeStart(BitWriter writer, AreaOfEffectExecutionParameters parameters)
        {
            if (!parameters.Context.Associate.TryGetComponent<BaseCombatAiComponent>(out var baseCombatAiComponent))
                return;

            var validTarget = baseCombatAiComponent.SeekValidTargets();
            var sourcePosition = parameters.NpcContext.CalculatingPosition;

            var targets = validTarget.Where(target =>
            {
                var transform = target.Transform;
                var distance = Vector3.Distance(transform.Position, sourcePosition);
                var valid = distance <= Radius;
                return valid;
            }).ToArray();

            if (targets.Length > 0)
                parameters.NpcContext.FoundTarget = true;

            // Write all target ids
            writer.Write((uint) targets.Length);
            foreach (var target in targets)
            {
                writer.Write(target);
            }

            foreach (var target in targets)
            {
                var behaviorBase = Action.SerializeStart(writer, parameters.NpcContext, 
                    new ExecutionBranchContext()
                    {
                        Target = target,
                        Duration = parameters.BranchContext.Duration
                    });
                
                parameters.TargetActions.Add(behaviorBase);
            }
        }
    }
}