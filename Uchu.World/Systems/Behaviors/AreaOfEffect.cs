using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Uchu.Core;

namespace Uchu.World.Systems.Behaviors
{
    public class AreaOfEffectExecutionParameters : BehaviorExecutionParameters
    {
        public uint Length { get; set; }
        public List<BehaviorExecutionParameters> TargetActions { get; } = 
            new List<BehaviorExecutionParameters>();
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

        protected override void DeserializeStart(AreaOfEffectExecutionParameters behaviorExecutionParameters)
        {
            behaviorExecutionParameters.Length = behaviorExecutionParameters.Context.Reader.Read<uint>();
            var targets = new List<GameObject>();
            for (var i = 0; i < behaviorExecutionParameters.Length; i++)
            {
                var targetId = behaviorExecutionParameters.Context.Reader.Read<long>();
                if (!behaviorExecutionParameters.Context.Associate.Zone.TryGetGameObject(targetId, 
                    out var target))
                {
                    Logger.Error(
                        $"{behaviorExecutionParameters.Context.Associate} sent invalid AreaOfEffect target.");
                    continue;
                }
                targets.Add(target);
            }
            
            Logger.Debug($"Found {behaviorExecutionParameters.Length} targets");
            Logger.Debug(targets);

            foreach (var target in targets)
            {
                var behaviorBase = Action.DeserializeStart(behaviorExecutionParameters.Context, 
                    new ExecutionBranchContext()
                    {
                        Target = target,
                        Duration = behaviorExecutionParameters.BranchContext.Duration
                    });
                
                behaviorExecutionParameters.TargetActions.Add(behaviorBase);
            }
        }

        protected override Task ExecuteStart(AreaOfEffectExecutionParameters behaviorExecutionsParameters)
        {
            foreach (var behaviorExecutionParameters in behaviorExecutionsParameters.TargetActions)
            {
                // Run in the background to avoid waiting for each other
                Task.Run(async () =>
                {
                    await Action.ExecuteStart(behaviorExecutionParameters);
                });
            }

            return Task.CompletedTask;
        }

        protected override void SerializeStart(AreaOfEffectExecutionParameters parameters)
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
            parameters.NpcContext.Writer.Write((uint) targets.Length);
            foreach (var target in targets)
            {
                parameters.NpcContext.Writer.Write(target);
            }

            foreach (var target in targets)
            {
                var behaviorBase = Action.SerializeStart(parameters.NpcContext, 
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