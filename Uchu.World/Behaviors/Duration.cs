using System.Threading.Tasks;
using RakDotNet.IO;

namespace Uchu.World.Behaviors
{
    public class Duration : Behavior
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.Duration;

        public override async Task Serialize(BitReader reader)
        {
            var action = await GetParameter(BehaviorId, "action");

            if (action.Value != null) await StartBranch((int) action.Value, reader);
        }
    }
}