using RakDotNet.IO;
using Uchu.World.Parsers;

namespace Uchu.World
{
    public class Component107 : ReplicaComponent
    {
        public override ComponentId Id => ComponentId.Component107;

        public override void FromLevelObject(LevelObject levelObject)
        {
        }

        public override void Construct(BitWriter writer)
        {
            Serialize(writer);
        }

        public override void Serialize(BitWriter writer)
        {
            writer.WriteBit(false);
        }
    }
}