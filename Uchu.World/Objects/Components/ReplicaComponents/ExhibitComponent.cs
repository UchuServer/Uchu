using RakDotNet.IO;
using Uchu.World.Parsers;

namespace Uchu.World
{
    public class ExhibitComponent : ReplicaComponent
    {
        public override ReplicaComponentsId Id => ReplicaComponentsId.Exhibit;

        public override void FromLevelObject(LevelObject levelObject)
        {
        }

        public override void Construct(BitWriter writer)
        {
            Serialize(writer);
        }

        public override void Serialize(BitWriter writer)
        {
            writer.WriteBit(true);
            writer.Write(GameObject.Lot);
        }
    }
}