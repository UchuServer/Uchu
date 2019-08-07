using RakDotNet.IO;
using Uchu.World.Parsers;

namespace Uchu.World
{
    public class SpawnerComponent : ReplicaComponent
    {
        public override ReplicaComponentsId Id => ReplicaComponentsId.Spawner;
        
        public override void FromLevelObject(LevelObject levelObject)
        {
            
        }

        public override void Construct(BitWriter writer)
        {
        }

        public override void Serialize(BitWriter writer)
        {
        }
    }
}