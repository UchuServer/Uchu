using RakDotNet.IO;

namespace Uchu.World
{
    [Essential]
    [RequireComponent(typeof(Stats), true)]
    public class Collectible : ReplicaComponent
    {
        public ushort CollectibleId { get; set; }
        
        public override ReplicaComponentsId Id => ReplicaComponentsId.Collectible;
        
        public override void Construct(BitWriter writer)
        {
            Serialize(writer);
        }

        public override void Serialize(BitWriter writer)
        {
            writer.Write(CollectibleId);
        }
    }
}