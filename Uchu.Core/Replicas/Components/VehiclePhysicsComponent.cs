using RakDotNet;

namespace Uchu.Core
{
    public class VehiclePhysicsComponent : ReplicaComponent
    {
        public override void Serialize(BitStream stream)
        {
            stream.WriteBit(false);
        }

        public override void Construct(BitStream stream)
        {
            stream.WriteByte(0);
            stream.WriteBit(false);

            Serialize(stream);
        }
    }
}