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
            writer.Write(false);
            writer.Write(false);
        }

        public override void Serialize(BitWriter writer)
        {
            // Empty
        }
    }
}