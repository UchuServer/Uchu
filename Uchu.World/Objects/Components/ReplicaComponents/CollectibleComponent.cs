using RakDotNet.IO;
using Uchu.World.Parsers;

namespace Uchu.World
{
    [Essential]
    [RequireComponent(typeof(StatsComponent), true)]
    public class CollectibleComponent : ReplicaComponent
    {
        public ushort CollectibleId { get; set; }
        
        public override ReplicaComponentsId Id => ReplicaComponentsId.Collectible;

        public override void FromLevelObject(LevelObject levelObject)
        {
            CollectibleId = (ushort) (int) levelObject.Settings["collectible_id"];
        }

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