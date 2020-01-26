using System.Numerics;
using RakDotNet.IO;

namespace Uchu.World
{
    public class PhantomPhysicsComponent : ReplicaComponent
    {
        public override ComponentId Id => ComponentId.PhantomPhysicsComponent;

        public bool HasPosition { get; set; } = true;

        public bool IsEffectActive { get; set; }

        public uint EffectType { get; set; }

        public float EffectAmount { get; set; }
        
        public bool AffectedByDistance { get; set; }

        public float MinDistance { get; set; }
        
        public float MaxDistance { get; set; }
        
        public Vector3 EffectDirection { get; set; }
        
        public override void Construct(BitWriter writer)
        {
            Serialize(writer);
        }

        public override void Serialize(BitWriter writer)
        {
            if (writer.Flag(HasPosition))
            {
                writer.Write(Transform.Position);
                writer.Write(Transform.Rotation);
            }

            writer.WriteBit(true);
            
            if (!writer.Flag(IsEffectActive)) return;

            writer.Write(EffectType);
            writer.Write(EffectAmount);

            if (writer.Flag(AffectedByDistance))
            {
                writer.Write(MinDistance);
                writer.Write(MaxDistance);
            }

            if (!writer.Flag(EffectDirection != Vector3.Zero)) return;

            writer.Write(EffectDirection * EffectAmount);
        }
    }
}