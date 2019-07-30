using RakDotNet.IO;

namespace Uchu.World
{
    [Essential]
    public class RigidBodyPhantomPhysics : ReplicaComponent
    {
        public override ReplicaComponentsId Id => ReplicaComponentsId.RigidBodyPhantomPhysics;
        
        public override void Construct(BitWriter writer)
        {
            Serialize(writer);
        }

        public override void Serialize(BitWriter writer)
        {
            writer.WriteBit(true);
            writer.Write(Transform.Position);
            writer.Write(Transform.Rotation);
        }
    }
}