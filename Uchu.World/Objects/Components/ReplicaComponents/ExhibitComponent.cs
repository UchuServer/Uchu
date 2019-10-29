using RakDotNet.IO;

namespace Uchu.World
{
    public class ExhibitComponent : ReplicaComponent
    {
        public override ComponentId Id => ComponentId.ExhibitComponent;

        public override void Construct(BitWriter writer)
        {
            Serialize(writer);
        }

        public override void Serialize(BitWriter writer)
        {
            writer.WriteBit(true);
            writer.Write(GameObject.Lot);
        }
    }
}