using RakDotNet.IO;
using Uchu.World.Parsers;

namespace Uchu.World
{
    [Essential]
    [RequireComponent(typeof(StatsComponent))]
    public class DestructibleComponent : ReplicaComponent
    {
        public override ReplicaComponentsId Id => ReplicaComponentsId.Destructible;

        public override void FromLevelObject(LevelObject levelObject)
        {
            
        }

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