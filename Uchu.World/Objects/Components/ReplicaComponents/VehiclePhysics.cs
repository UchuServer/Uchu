using RakDotNet.IO;

namespace Uchu.World
{
    [Essential]
    public class VehiclePhysics : ReplicaComponent
    {
        public override ReplicaComponentsId Id => ReplicaComponentsId.VehiclePhysics;
        
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