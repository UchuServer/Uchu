using System.Numerics;
using RakDotNet.IO;

namespace Uchu.World
{
    [Essential]
    public class PhantomPhysics : ReplicaComponent
    {
        public override ReplicaComponentsId Id => ReplicaComponentsId.PhantomPhysics;

        public bool IsEffectActive { get; set; } = false;
        
        public uint EffectType { get; set; }
        
        public float EffectAmount { get; set; }

        public Vector3? EffectDirection { get; set; } = null;
        
        public override void Construct(BitWriter writer)
        {
            Serialize(writer);
        }

        public override void Serialize(BitWriter writer)
        {
            writer.WriteBit(true);

            writer.Write(Transform.Position);
            writer.Write(Transform.Rotation);

            writer.WriteBit(true);
            writer.WriteBit(IsEffectActive);
            
            if (!IsEffectActive) return;

            writer.Write(EffectType);
            writer.Write(EffectAmount);
            writer.WriteBit(false);

            var hasDirection = EffectDirection != null;

            writer.WriteBit(hasDirection);
            
            if (!hasDirection) return;

            writer.Write(EffectDirection.Value * EffectAmount);
        }
    }
}