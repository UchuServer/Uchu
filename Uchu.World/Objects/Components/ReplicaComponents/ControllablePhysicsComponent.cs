using System.Numerics;
using RakDotNet.IO;
using Uchu.World.Parsers;

namespace Uchu.World
{
    public class ControllablePhysicsComponent : ReplicaComponent
    {
        public uint JetpackEffectId { get; set; }

        public bool HasPosition { get; set; } = true;

        public bool IsOnGround { get; set; } = true;

        public bool NegativeAngularVelocity { get; set; }

        public bool HasVelocity { get; set; }

        public Vector3 Velocity { get; set; }

        public bool HasAngularVelocity { get; set; }

        public Vector3 AngularVelocity { get; set; }

        public GameObject Platform { get; set; }

        public Vector3 PlatformPosition { get; set; } = Vector3.Zero;

        public override ComponentId Id => ComponentId.ControllablePhysics;

        public override void FromLevelObject(LevelObject levelObject)
        {
        }

        public override void Construct(BitWriter writer)
        {
            var hasJetpackEffect = JetpackEffectId != 0;

            writer.WriteBit(hasJetpackEffect);

            if (hasJetpackEffect)
            {
                writer.Write(JetpackEffectId);
                writer.WriteBit(false); // Is in air?
                writer.WriteBit(false);
            }

            writer.WriteBit(true);

            for (var i = 0; i < 7; i++) writer.Write<uint>(0);

            WritePhysics(writer);
        }

        public override void Serialize(BitWriter writer)
        {
            WritePhysics(writer);

            writer.WriteBit(false);
        }

        public void WritePhysics(BitWriter writer)
        {
            writer.WriteBit(false);

            writer.WriteBit(true);
            writer.Write<float>(0);
            writer.WriteBit(false);

            writer.WriteBit(true);
            writer.WriteBit(false);

            writer.WriteBit(HasPosition);

            if (!HasPosition) return;

            writer.Write(Transform.Position);
            writer.Write(Transform.Rotation);

            writer.WriteBit(IsOnGround);
            writer.WriteBit(NegativeAngularVelocity);

            writer.WriteBit(HasVelocity);

            if (HasVelocity) writer.Write(Velocity);

            writer.WriteBit(HasAngularVelocity);

            if (HasAngularVelocity) writer.Write(AngularVelocity);

            var hasPlatform = Platform != null;

            writer.WriteBit(hasPlatform);

            if (!hasPlatform) return;

            writer.Write(Platform);

            writer.Write(PlatformPosition);

            writer.WriteBit(false);
        }
    }
}