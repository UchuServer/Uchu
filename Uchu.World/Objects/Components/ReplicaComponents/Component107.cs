using RakDotNet.IO;

namespace Uchu.World
{
    [Essential]
    public class Component107 : ReplicaComponent
    {
        public override ReplicaComponentsId Id => ReplicaComponentsId.Component107;
        
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