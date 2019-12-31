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
        
        public override async Task BuildAsync()
        {
            CheckEnvironment = (await GetParameter("check_env")).Value > 0;
            Blocked = await GetParameter("blocked action") != default;

            ActionBehavior = await GetBehavior("action");
            BlockedBehavior = await GetBehavior("blocked action");
            MissBehavior = await GetBehavior("miss action");
        }

        public override async Task ExecuteAsync(ExecutionContext context, ExecutionBranchContext branchContext)
        {
            await base.ExecuteAsync(context, branchContext);
            
            if (context.Reader.ReadBit()) // Hit
            {
                if (CheckEnvironment)
                    context.Reader.ReadBit(); // Check environment
                
                var targets = new GameObject[context.Reader.Read<uint>()];

                for (var i = 0; i < targets.Length; i++)
                {
                    var targetId = context.Reader.Read<long>();
                    
                    if (!context.Associate.Zone.TryGetGameObject(targetId, out var target))
                    {
                        Logger.Error($"{context.Associate} sent invalid TacArc target: {targetId}");
                        
                        continue;
                    }
                    
                    targets[i] = target;
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
                    if (context.Reader.ReadBit()) // Is blocked
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
    }
}