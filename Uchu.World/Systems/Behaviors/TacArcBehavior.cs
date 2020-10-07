using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Uchu.Core;

namespace Uchu.World.Systems.Behaviors
{
    public class TacArcBehaviorExecutionParameters : BehaviorExecutionParameters
    {
        public bool Hit { get; set; }
        public bool Blocked { get; set; }
        public BehaviorBase Behavior { get; set; }
        public BehaviorExecutionParameters Parameters { get; set; }
        public List<BehaviorExecutionParameters> ParametersList { get; set; } 
            = new List<BehaviorExecutionParameters>();
    }
    public class TacArcBehavior : BehaviorBase<TacArcBehaviorExecutionParameters>
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.TacArc;

        private bool CheckEnvironment { get; set; }
        private bool Blocked { get; set; }
        private BehaviorBase ActionBehavior { get; set; }
        private BehaviorBase BlockedBehavior { get; set; }
        private BehaviorBase MissBehavior { get; set; }
        private int MaxTargets { get; set; }
        private bool UsePickedTarget { get; set; }
        
        public override async Task BuildAsync()
        {
            CheckEnvironment = (await GetParameter("check_env"))?.Value > 0;
            Blocked = await GetParameter("blocked action") != default;

            ActionBehavior = await GetBehavior("action");
            BlockedBehavior = await GetBehavior("blocked action");
            MissBehavior = await GetBehavior("miss action");

            MaxTargets = await GetParameter<int>("max targets");
            UsePickedTarget = await GetParameter<int>("use_picked_target") > 0;
        }

        protected override void DeserializeStart(TacArcBehaviorExecutionParameters parameters)
        {
            if (UsePickedTarget && parameters.BranchContext.Target != null)
            {
                parameters.Behavior = ActionBehavior;
                parameters.Parameters = parameters.Behavior.DeserializeStart(parameters.Context,
                    parameters.BranchContext);
            }
            else
            {
                parameters.Hit = parameters.Context.Reader.ReadBit();
                if (CheckEnvironment)
                {
                    parameters.Blocked = parameters.Context.Reader.ReadBit();
                    if (parameters.Blocked)
                    {
                        parameters.Behavior = BlockedBehavior;
                        parameters.Parameters = parameters.Behavior.DeserializeStart(
                            parameters.Context, parameters.BranchContext);
                        return;
                    }
                }

                if (parameters.Hit)
                {
                    parameters.Behavior = ActionBehavior;
                    DeserializeBehaviorForMultipleTargets(parameters);
                }
                else
                {
                    parameters.Behavior = MissBehavior;
                    parameters.Parameters = parameters.Behavior.DeserializeStart(
                        parameters.Context, parameters.BranchContext);
                }
            }
        }

        protected override async Task ExecuteStart(TacArcBehaviorExecutionParameters parameters)
        {
            if (parameters.ParametersList.Count > 0)
            {
                foreach (var parameter in parameters.ParametersList)
                {
                    await parameters.Behavior.ExecuteStart(parameter);
                }
            }
            else
            {
                await parameters.Behavior.ExecuteStart(parameters.Parameters);
            }
        }

        protected override async Task ExecuteSync(TacArcBehaviorExecutionParameters parameters)
        {
            if (parameters.ParametersList.Count > 0)
            {
                foreach (var parameter in parameters.ParametersList)
                {
                    await parameters.Behavior.ExecuteSync(parameter);
                }
            }
            else
            {
                await parameters.Behavior.ExecuteSync(parameters.Parameters);
            }
        }

        protected override void SerializeSync(TacArcBehaviorExecutionParameters parameters)
        {
            if (parameters.ParametersList.Count > 0)
            {
                foreach (var parameter in parameters.ParametersList)
                {
                    parameters.Behavior.SerializeSync(parameter);
                }
            }
            else
            {
                parameters.Behavior.SerializeSync(parameters.Parameters);
            }
        }

        /// <summary>
        /// Finds all targets in the area and optionally hits them if not missed and not blocked
        /// </summary>
        /// <param name="parameters">The parameters to find and hit targets for</param>
        /// <returns>A list of the execution parameters to run</returns>
        private void DeserializeBehaviorForMultipleTargets(
            TacArcBehaviorExecutionParameters parameters)
        {
            var targetCount = parameters.Context.Reader.Read<uint>();
            var targets = new List<GameObject>();

            // Find all targets for this hit
            for (var i = 0; i < targetCount; i++)
            {
                var targetId = parameters.Context.Reader.Read<long>();
                if (!parameters.Context.Associate.Zone.TryGetGameObject(targetId, out var target))
                {
                    Logger.Error($"{parameters.Context.Associate} sent invalid TacArc target: {targetId}");
                    continue;
                }
                targets.Add(target);
            }

            // Hit all targets
            foreach (var target in targets)
            {
                var res = parameters.Behavior.DeserializeStart(parameters.Context,
                    new ExecutionBranchContext
                    {
                        Duration = parameters.BranchContext.Duration,
                        Target = target
                    });
                parameters.ParametersList.Add(res);
            }
        }

        protected override void SerializeStart(TacArcBehaviorExecutionParameters parameters)
        {
            if (!(parameters.NpcContext.Associate.TryGetComponent<BaseCombatAiComponent>(out var combatAi) 
                || parameters.NpcContext.Alive))
                return;

            // If there's a single target to hit, hit them and exit
            if (UsePickedTarget && parameters.BranchContext.Target != null)
            {
                parameters.Behavior = ActionBehavior;
                parameters.Parameters = parameters.Behavior.SerializeStart(parameters.NpcContext,
                    parameters.BranchContext);
            }
            else
            {
                var targets = LocateTargets(parameters.NpcContext);
                var any = targets.Any();
                parameters.NpcContext.Writer.WriteBit(any);
                
                // If we have to check the environment, check if this is a blocked action and notify all the targets
                if (CheckEnvironment)
                {
                    parameters.NpcContext.Writer.WriteBit(Blocked);
                    if (Blocked)
                    {
                        parameters.Behavior = BlockedBehavior;
                        foreach (var target in targets)
                        {
                            var targetParameters = parameters.Behavior.SerializeStart(parameters.NpcContext,
                                    new ExecutionBranchContext()
                                    {
                                        Duration = parameters.BranchContext.Duration,
                                        Target = target
                                    });
                            parameters.ParametersList.Add(targetParameters);
                        }
                        return;
                    }
                }
                
                // If there was no target, find targets if the game object is alive
                if (any)
                {
                    combatAi.Target = targets.First();
                    parameters.NpcContext.FoundTarget = true;
                    parameters.NpcContext.Writer.Write((uint) targets.Count);

                    // Register all targets
                    foreach (var target in targets)
                    {
                        parameters.NpcContext.Writer.Write((long)target.Id);
                    }
                    
                    // Hit all targets
                    parameters.Behavior = ActionBehavior;
                    foreach (var target in targets)
                    {
                        var targetParameters = parameters.Behavior.SerializeStart(parameters.NpcContext,
                                new ExecutionBranchContext()
                                {
                                    Duration = parameters.BranchContext.Duration,
                                    Target = target
                                });
                        parameters.ParametersList.Add(targetParameters);
                    }
                }
                else
                {
                    parameters.Behavior = MissBehavior;
                    parameters.Parameters = parameters.Behavior.SerializeStart(parameters.NpcContext,
                        parameters.BranchContext);
                }
            }
        }

        /// <summary>
        /// Locates all targets within range of a context and returns a list of them
        /// </summary>
        /// <param name="context">The NPC context to find targets for</param>
        /// <returns>A list of targets if they were found, otherwise an empty list</returns>
        private List<GameObject> LocateTargets(NpcExecutionContext context)
        {
            if (!context.Associate.TryGetComponent<BaseCombatAiComponent>(out var baseCombatAiComponent))
                return new List<GameObject>();

            var validTargets = baseCombatAiComponent.SeekValidTargets();
            var sourcePosition = context.CalculatingPosition; // Change back to author position?
            var targets = validTargets.Where(target =>
            {
                var transform = target.Transform;
                var distance = Vector3.Distance(transform.Position, sourcePosition);
                return distance <= context.MaxRange && context.MinRange <= distance;
            }).ToList();

            targets.ToList().Sort((g1, g2) =>
            {
                var distance1 = Vector3.Distance(g1.Transform.Position, sourcePosition);
                var distance2 = Vector3.Distance(g2.Transform.Position, sourcePosition);

                return (int) (distance1 - distance2);
            });

            var selectedTargets = new List<GameObject>();
            foreach (var target in targets)
            {
                if (selectedTargets.Count < MaxTargets)
                {
                    selectedTargets.Add(target);
                }
            }

            return selectedTargets;
        }
    }
}