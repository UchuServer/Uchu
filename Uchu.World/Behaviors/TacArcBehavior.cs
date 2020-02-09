using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Uchu.Core;

namespace Uchu.World.Behaviors
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
        
        public override async Task BuildAsync()
        {
            CheckEnvironment = (await GetParameter("check_env"))?.Value > 0;
            Blocked = await GetParameter("blocked action") != default;

            ActionBehavior = await GetBehavior("action");
            BlockedBehavior = await GetBehavior("blocked action");
            MissBehavior = await GetBehavior("miss action");

            MaxTargets = await GetParameter<int>("max targets");
        }

        public override async Task ExecuteAsync(ExecutionContext context, ExecutionBranchContext branchContext)
        {
            await base.ExecuteAsync(context, branchContext);

            var hit = context.Reader.ReadBit();
            context.Writer.Write(hit);
            
            if (hit) // Hit
            {
                if (CheckEnvironment)
                {
                    var checkEnvironment = context.Reader.ReadBit(); // Check environment

                    context.Writer.WriteBit(checkEnvironment);
                }

                var specifiedTargets = context.Reader.Read<uint>();

                context.Writer.Write(specifiedTargets);
                
                var targets = new List<GameObject>();

                for (var i = 0; i < specifiedTargets; i++)
                {
                    var targetId = context.Reader.Read<long>();

                    context.Writer.Write(targetId);
                    
                    if (!context.Associate.Zone.TryGetGameObject(targetId, out var target))
                    {
                        Logger.Error($"{context.Associate} sent invalid TacArc target: {targetId}");
                        
                        continue;
                    }
                    
                    targets.Add(target);
                }

                foreach (var target in targets)
                {
                    ((Player) context.Associate)?.SendChatMessage($"ATTACKING: {target}");

                    var branch = new ExecutionBranchContext(target)
                    {
                        Duration = branchContext.Duration
                    };

                    await ActionBehavior.ExecuteAsync(context, branch);
                }
            }
            else
            {
                if (Blocked)
                {
                    var isBlocked = context.Reader.ReadBit();

                    context.Writer.WriteBit(isBlocked);
                    
                    if (isBlocked) // Is blocked
                    {
                        await BlockedBehavior.ExecuteAsync(context, branchContext);
                    }
                    else
                    {
                        await MissBehavior.ExecuteAsync(context, branchContext);
                    }
                }
                else
                {
                    await MissBehavior.ExecuteAsync(context, branchContext);
                }
            }
        }

        public override async Task CalculateAsync(NpcExecutionContext context, ExecutionBranchContext branchContext)
        {
            if (!context.Associate.TryGetComponent<BaseCombatAiComponent>(out var baseCombatAiComponent)) return;

            var validTarget = await baseCombatAiComponent.SeekValidTargetsAsync();

            var sourcePosition = context.CalculatingPosition; // Change back to author position?

            var targets = validTarget.Where(target =>
            {
                var transform = target.Transform;

                var distance = Vector3.Distance(transform.Position, sourcePosition);

                return distance <= context.MaxRange && context.MinRange <= distance;
            }).ToArray();

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
                context.FoundTarget = true;

                if (CheckEnvironment)
                {
                    // TODO
                    context.Writer.WriteBit(false);
                }

                context.Writer.Write((uint) selectedTargets.Count);

                foreach (var target in selectedTargets)
                {
                    context.Writer.Write(target.ObjectId);
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