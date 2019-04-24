using System.Numerics;
using RakDotNet;

namespace Uchu.Core
{
    public class RigidBodyPhantomPhysicsComponent : ReplicaComponent, IPhysics
    {
        public bool HasPosition { get; set; } = false;
        public Vector3 Position { get; set; }
        public Vector4 Rotation { get; set; }

        public override void Serialize(BitStream stream)
        {
            stream.WriteBit(HasPosition);

            if (!HasPosition)
                return;

            stream.WriteFloat(Position.X);
            stream.WriteFloat(Position.Y);
            stream.WriteFloat(Position.Z);
            stream.WriteFloat(Rotation.X);
            stream.WriteFloat(Rotation.Y);
            stream.WriteFloat(Rotation.Z);
            stream.WriteFloat(Rotation.W);
        }

        public override void Construct(BitStream stream)
        {
            Serialize(stream);
        }
    }
}