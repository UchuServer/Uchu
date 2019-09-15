using RakDotNet.IO;
using Uchu.World.Parsers;

namespace Uchu.World
{
    public class ModelComponent : ReplicaComponent
    {
        // TODO: Look into this

        public override ReplicaComponentsId Id => ReplicaComponentsId.Model;

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