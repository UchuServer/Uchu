using System.Threading.Tasks;
using RakDotNet.IO;

namespace Uchu.World.Behaviors
{
    public class AttackDelay : Behavior
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.AttackDelay;

        public override Task SerializeAsync(BitReader reader)
        {
            // TODO

            return Task.CompletedTask;
        }
    }
}