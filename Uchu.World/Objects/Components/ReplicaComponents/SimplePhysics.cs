using RakDotNet.IO;

namespace Uchu.World
{
    [Essential]
    public class SimplePhysics : ReplicaComponent
    {
        public bool HasPosition { get; set; } = true;
        
        public override ReplicaComponentsId Id => ReplicaComponentsId.SimplePhysics;
        
        public override void Construct(BitWriter writer)
        {
            writer.WriteBit(false);
            writer.Write(0f);
            
            Serialize(writer);
        }

        public override void Serialize(BitWriter writer)
        {
            writer.WriteBit(false);
            writer.WriteBit(false);

            writer.Write(HasPosition);
            
            if (!HasPosition) return;

            writer.Write(Transform.Position);
            writer.Write(Transform.Rotation);
        }
    }
}