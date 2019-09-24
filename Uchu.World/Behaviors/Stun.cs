using System.Threading.Tasks;
using RakDotNet.IO;

namespace Uchu.World.Behaviors
{
    public class Stun : Behavior
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.Stun;

        public override Task SerializeAsync(BitReader reader)
        {
            // TODO

            return Task.CompletedTask;
        }
    }
}