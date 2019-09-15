using System.Threading.Tasks;
using RakDotNet.IO;

namespace Uchu.World.Behaviors
{
    public class AirMovement : Behavior
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.AirMovement;

        public override async Task Serialize(BitReader reader)
        {
            var handle = reader.Read<uint>();

            HandledBehaviors.TryAdd(handle, this);
        }
    }
}