using System.Linq;
using System.Threading.Tasks;

namespace Uchu.World.Behaviors
{
    public class AreaOfEffect : BehaviorBase
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.AreaOfEffect;
        
        public BehaviorBase Action { get; set; }
        
        public override async Task BuildAsync()
        {
            Action = await GetBehavior("action");
        }

        public override async Task ExecuteAsync(ExecutionContext context, ExecutionBranchContext branchContext)
        {
            await base.ExecuteAsync(context, branchContext);

            var length = context.Reader.Read<uint>();
            
            ((Player) context.Associate).SendChatMessage($"AREA LENGTH: {length}");

            if (length > 10) length = 10;

            context.Writer.Write(length);
            
            var targets = new GameObject[length];

            for (var i = 0; i < length; i++)
            {
                var id = context.Reader.Read<ulong>();

                context.Writer.Write(id);

                context.Associate.Zone.TryGetGameObject((long) id, out var target);
                
                ((Player) context.Associate)?.SendChatMessage($"AREA: {id} -> {target}");

                targets[i] = target;
            }
            
            foreach (var target in targets)
            {
                await Action.ExecuteAsync(context, new ExecutionBranchContext(target));
            }
        }
    }
}