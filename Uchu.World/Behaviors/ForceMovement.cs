using System.Threading.Tasks;
using RakDotNet.IO;

namespace Uchu.World.Behaviors
{
    public class ForceMovement : Behavior
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.ForceMovement;

        public override Task SerializeAsync(BitReader reader)
        {
            // TODO

            return Task.CompletedTask;
        }
    }
}