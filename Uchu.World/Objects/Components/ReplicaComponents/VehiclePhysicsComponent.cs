using RakDotNet.IO;
using Uchu.World.Parsers;

namespace Uchu.World
{
    public class VehiclePhysicsComponent : ReplicaComponent
    {
        public override ComponentId Id => ComponentId.VehiclePhysicsComponent;

        public override void Construct(BitWriter writer)
        {
            writer.Write<byte>(0);
            writer.WriteBit(false);

            Serialize(writer);
        }

        public override void Serialize(BitWriter writer)
        {
            writer.WriteBit(false);
        }
    }
}