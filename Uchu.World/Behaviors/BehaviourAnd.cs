using System.Threading.Tasks;
using RakDotNet.IO;

namespace Uchu.World.Behaviors
{
    public class BehaviourAnd : Behavior
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.And;

        public override async Task SerializeAsync(BitReader reader)
        {
            var behaviors = GetParameters(BehaviorId);

            foreach (var behavior in behaviors)
            {
                if (behavior.Value == null) continue;

                await StartBranch((int) behavior.Value, reader);
            }
        }
    }
}