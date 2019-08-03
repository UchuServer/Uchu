using RakDotNet.IO;

namespace Uchu.World
{
    [Essential]
    [RequireComponent(typeof(Stats))]
    public class Destructible : ReplicaComponent
    {
        public override ReplicaComponentsId Id => ReplicaComponentsId.Destructible;
        
        public override void Construct(BitWriter writer)
        {
            writer.WriteBit(false);
            writer.WriteBit(false);
        }

        public override void Serialize(BitWriter writer)
        {
            // Empty
        }
    }
}