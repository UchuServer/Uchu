using System.Numerics;
using RakDotNet;

namespace Uchu.Core
{
    public class ControllablePhysicsComponent : ReplicaComponent, IPhysics
    {
        public uint JetpackEffectId { get; set; } = 0;

        public bool HasPosition { get; set; } = false;

        public Vector3 Position { get; set; }

        public Vector4 Rotation { get; set; }

        public bool IsOnGround { get; set; } = true;
        public bool NegativeAngularVelocity { get; set; } = false;

        public Vector3? Velocity { get; set; } = null;

        public Vector3? AngularVelocity { get; set; } = null;

        public long PlatformObjectId { get; set; } = -1;

        public Vector3 PlatformPosition { get; set; } = Vector3.Zero;

        private void _write(BitStream stream)
        {
            stream.WriteBit(false);

            stream.WriteBit(true);
            stream.WriteFloat(0);
            stream.WriteBit(false);

            stream.WriteBit(true);
            stream.WriteBit(false);

            stream.WriteBit(HasPosition);

            if (HasPosition)
            {
                stream.WriteFloat(Position.X);
                stream.WriteFloat(Position.Y);
                stream.WriteFloat(Position.Z);

                stream.WriteFloat(Rotation.X);
                stream.WriteFloat(Rotation.Y);
                stream.WriteFloat(Rotation.Z);
                stream.WriteFloat(Rotation.W);

                stream.WriteBit(IsOnGround);
                stream.WriteBit(NegativeAngularVelocity);

                var hasVelocity = Velocity != null;

                stream.WriteBit(hasVelocity);

                if (hasVelocity)
                {
                    var vec = (Vector3) Velocity;

                    stream.WriteFloat(vec.X);
                    stream.WriteFloat(vec.Y);
                    stream.WriteFloat(vec.Z);
                }

                var hasAngVelocity = AngularVelocity != null;

                stream.WriteBit(hasAngVelocity);

                if (hasAngVelocity)
                {
                    var vec = (Vector3) AngularVelocity;

                    stream.WriteFloat(vec.X);
                    stream.WriteFloat(vec.Y);
                    stream.WriteFloat(vec.Z);
                }

                var hasPlatform = PlatformObjectId != -1;

                stream.WriteBit(hasPlatform);

                if (hasPlatform)
                {
                    stream.WriteLong(PlatformObjectId);

                    stream.WriteFloat(PlatformPosition.X);
                    stream.WriteFloat(PlatformPosition.Y);
                    stream.WriteFloat(PlatformPosition.Z);

                    stream.WriteBit(false);
                }
            }
        }

        public override void Serialize(BitStream stream)
        {
            _write(stream);

            stream.WriteBit(false);
        }

        public override void Construct(BitStream stream)
        {
            var hasJetpackEffect = JetpackEffectId != 0;

            stream.WriteBit(hasJetpackEffect);

            if (hasJetpackEffect)
            {
                stream.WriteUInt(JetpackEffectId);
                stream.WriteBit(false);
            }

            stream.WriteBit(true);

            for (var i = 0; i < 7; i++)
                stream.WriteUInt(0);

            _write(stream);
        }
    }
}