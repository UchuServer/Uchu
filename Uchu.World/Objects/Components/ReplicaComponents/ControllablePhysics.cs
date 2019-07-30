using System.Numerics;
using RakDotNet.IO;

namespace Uchu.World
{
    [Essential]
    public class ControllablePhysics : ReplicaComponent
    {
        public uint JetpackEffectId { get; set; }
        
        public bool HasPosition { get; set; }
        
        public bool IsOnGround { get; set; }
        
        public bool NegativeAngularVelocity { get; set; }
        
        public Vector3? Velocity { get; set; }

        public Vector3? AngularVelocity { get; set; }

        public GameObject Platform { get; set; }

        public Vector3 PlatformPosition { get; set; } = Vector3.Zero;
        
        public override ReplicaComponentsId Id => ReplicaComponentsId.ControllablePhysics;
        
        public override void Construct(BitWriter writer)
        {
            var hasJetpackEffect = JetpackEffectId != 0;

            writer.WriteBit(hasJetpackEffect);

            if (hasJetpackEffect)
            {
                writer.Write(JetpackEffectId);
                writer.WriteBit(false);
                writer.WriteBit(false);
            }

            writer.Write(true);
            writer.Write(new byte[7 * 4], 7 * 4 * 8);

            WritePhysics(writer);
        }

        public override void Serialize(BitWriter writer)
        {
            WritePhysics(writer);

            writer.Write(false);
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

            var hasVelocity = Velocity != null;

            if (hasVelocity) writer.Write(Velocity.Value);

            var hasAngularVelocity = AngularVelocity != null;

            if (hasAngularVelocity) writer.Write(AngularVelocity.Value * (NegativeAngularVelocity ? -1 : 1));

            var hasPlatform = Platform != null;

            writer.Write(hasPlatform);
            
            if (!hasPlatform) return;

            writer.Write(Platform);

            writer.Write(PlatformPosition);

            writer.WriteBit(false);
        }
    }
}