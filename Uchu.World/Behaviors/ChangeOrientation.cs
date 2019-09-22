using System.Threading.Tasks;
using RakDotNet.IO;

namespace Uchu.World.Behaviors
{
    public class ChangeOrientation : Behavior
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.ChangeOrientation;

        public override Task SerializeAsync(BitReader reader)
        {
            // TODO

            return Task.CompletedTask;
        }
    }
}