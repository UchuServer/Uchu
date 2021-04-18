using System.Numerics;
using RakDotNet.IO;

namespace Uchu.World
{
    public class ControllablePhysicsComponent : ReplicaComponent
    {
        public uint JetpackEffectId { get; set; }

        public bool Flying { get; set; }

        public float JetPackAirSpeed { get; set; }

        public bool HasPosition { get; set; } = true;

        public bool IsOnGround { get; set; } = true;

        public bool NegativeAngularVelocity { get; set; }

        public bool HasVelocity { get; set; }

        public Vector3 Velocity { get; set; }

        public bool HasAngularVelocity { get; set; }

        public Vector3 AngularVelocity { get; set; }

        public GameObject Platform { get; set; }

        public Vector3 PlatformPosition { get; set; }

        public float GravityMultiplier { get; set; } = 1;

        public float SpeedMultiplier { get; set; } = 1;

        public override ComponentId Id => ComponentId.ControllablePhysicsComponent;
        
        public override void Construct(BitWriter writer)
        {
            var hasJetpackEffect = JetpackEffectId != 0;

            writer.WriteBit(hasJetpackEffect);

            if (hasJetpackEffect)
            {
                writer.Write(JetpackEffectId);
                writer.WriteBit(Flying);
                writer.WriteBit(false); // Bypass Checks?
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
            if (writer.Flag(!GravityMultiplier.Equals(1) || !SpeedMultiplier.Equals(1)))
            {
                writer.Write(GravityMultiplier);
                writer.Write(SpeedMultiplier);
            }
            
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