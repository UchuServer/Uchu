using RakDotNet.IO;

namespace Uchu.World
{
    public class Vendor : ReplicaComponent
    {
        public override ReplicaComponentsId Id => ReplicaComponentsId.Vendor;
        
        public override void Construct(BitWriter writer)
        {
            Serialize(writer);
        }

        public override void Serialize(BitWriter writer)
        {
            writer.WriteBit(false);
        }
    }
}