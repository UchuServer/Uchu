using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.World.Systems.Behaviors
{
    public class TacArcBehavior : BehaviorBase
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.TacArc;
        
        public bool CheckEnvironment { get; set; }

        public bool Blocked { get; set; }
        
        public BehaviorBase ActionBehavior { get; set; }
        
        public BehaviorBase BlockedBehavior { get; set; }
        
        public BehaviorBase MissBehavior { get; set; }
        
        public int MaxTargets { get; set; }
        
        public bool UsePickedTarget { get; set; }
        
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

        public override async Task ExecuteAsync(ExecutionContext context, ExecutionBranchContext branchContext)
        {
            await base.ExecuteAsync(context, branchContext);

            if (UsePickedTarget && context.ExplicitTarget != null)
            {
                var branch = new ExecutionBranchContext(context.ExplicitTarget)
                {
                    Duration = branchContext.Duration
                };
                
                await ActionBehavior.ExecuteAsync(context, branch);
            }
            else
            {
                var hit = context.Reader.ReadBit();
                if (CheckEnvironment)
                {
                    var blocked = context.Reader.ReadBit();
                    if (blocked)
                    {
                        await BlockedBehavior.ExecuteAsync(context, branchContext);
                        return;
                    }
                }
                
                if (hit)
                {
                    await FindAndHitTargets(context, branchContext);
                }
                else
                {
                    await MissBehavior.ExecuteAsync(context, branchContext);
                }
            }
        }

        /// <summary>
        /// Finds all targets in the area and optionally hits them if not missed and not blocked
        /// </summary>
        /// <param name="context">The context to find the targets from</param>
        /// <param name="branchContext">The branch context to execute further behaviors from</param>
        private async Task FindAndHitTargets(ExecutionContext context, ExecutionBranchContext branchContext)
        {
            var targetCount = context.Reader.Read<uint>();
            var targets = new List<GameObject>();

            // Find all targets for this hit
            for (var i = 0; i < targetCount; i++)
            {
                var targetId = context.Reader.Read<long>();
                if (!context.Associate.Zone.TryGetGameObject(targetId, out var target))
                {
                    Logger.Error($"{context.Associate} sent invalid TacArc target: {targetId}");
                    continue;
                }
                targets.Add(target);
            }

            // Hit all targets
            foreach (var target in targets)
            {
                var branch = new ExecutionBranchContext(target)
                {
                    Duration = branchContext.Duration
                };

                await ActionBehavior.ExecuteAsync(context, branch);
            }
        }

        public override async Task CalculateAsync(NpcExecutionContext context, ExecutionBranchContext branchContext)
        {
            if (!context.Associate.TryGetComponent<BaseCombatAiComponent>(out var baseCombatAiComponent)) return;

            var validTarget = baseCombatAiComponent.SeekValidTargets();
            var sourcePosition = context.CalculatingPosition; // Change back to author position?
            var targets = validTarget.Where(target =>
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

            if (!context.Alive)
            {
                selectedTargets.Clear(); // No targeting if dead
            }

            var any = selectedTargets.Any();

            context.Writer.WriteBit(any); // Hit

            if (any)
            {
                baseCombatAiComponent.Target = selectedTargets.First();
                
                context.FoundTarget = true;

                if (CheckEnvironment)
                {
                    // TODO
                    context.Writer.WriteBit(false);
                }

                context.Writer.Write((uint) selectedTargets.Count);

                foreach (var target in selectedTargets)
                {
                    context.Writer.Write(target.Id);
                }

                foreach (var target in selectedTargets)
                {
                    if (!(target is Player player)) continue;

                    player.SendChatMessage($"You are a target! [{context.SkillSyncId}]");
                }

                foreach (var target in selectedTargets)
                {
                    var branch = new ExecutionBranchContext(target)
                    {
                        Duration = branchContext.Duration
                    };

                    await ActionBehavior.CalculateAsync(context, branch);
                }
            }
            else
            {
                if (Blocked)
                {
                    // TODO
                    context.Writer.WriteBit(false);
                }
                else
                {
                    await MissBehavior.CalculateAsync(context, branchContext);
                }
            }
        }
    }
}