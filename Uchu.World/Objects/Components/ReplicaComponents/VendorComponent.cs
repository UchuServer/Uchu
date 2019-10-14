using RakDotNet.IO;
using Uchu.World.Parsers;

namespace Uchu.World
{
    public class VendorComponent : ReplicaComponent
    {
        public override ComponentId Id => ComponentId.Vendor;

        public override void FromLevelObject(LevelObject levelObject)
        {
        }

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