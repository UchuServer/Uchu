using System.Numerics;
using RakDotNet;

namespace Uchu.Core
{
    public class PhantomPhysicsComponent : ReplicaComponent
    {
        public Vector3 Position { get; set; }
        public Vector4 Rotation { get; set; }

        public bool IsEffectActive { get; set; } = false;

        public uint EffectType { get; set; }
        public float EffectAmount { get; set; }

        public Vector3? EffectDirection { get; set; } = null;

        public override void Serialize(BitStream stream)
        {
            stream.WriteBit(true);
            stream.WriteFloat(Position.X);
            stream.WriteFloat(Position.Y);
            stream.WriteFloat(Position.Z);
            stream.WriteFloat(Rotation.X);
            stream.WriteFloat(Rotation.Y);
            stream.WriteFloat(Rotation.Z);
            stream.WriteFloat(Rotation.W);

            stream.WriteBit(true);
            stream.WriteBit(IsEffectActive);

            if (IsEffectActive)
            {
                stream.WriteUInt(EffectType);
                stream.WriteFloat(EffectAmount);
                stream.WriteBit(false);

                var hasDirection = EffectDirection != null;

                stream.WriteBit(hasDirection);

                if (hasDirection)
                {
                    var vec = (Vector3) EffectDirection;

                    stream.WriteFloat(vec.X * EffectAmount);
                    stream.WriteFloat(vec.Y * EffectAmount);
                    stream.WriteFloat(vec.Z * EffectAmount);
                }
            }
        }

        public override void Construct(BitStream stream)
        {
            Serialize(stream);
        }
    }
}