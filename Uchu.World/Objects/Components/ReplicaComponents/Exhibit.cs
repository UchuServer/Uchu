using RakDotNet.IO;

namespace Uchu.World
{
    [Essential]
    public class Exhibit : ReplicaComponent
    {
        public override ReplicaComponentsId Id => ReplicaComponentsId.Exhibit;
        
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