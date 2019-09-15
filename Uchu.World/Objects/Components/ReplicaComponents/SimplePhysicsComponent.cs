using RakDotNet.IO;
using Uchu.World.Parsers;

namespace Uchu.World
{
    public class SimplePhysicsComponent : ReplicaComponent
    {
        public bool HasPosition { get; set; } = true;

        public override ReplicaComponentsId Id => ReplicaComponentsId.SimplePhysics;

        public override void FromLevelObject(LevelObject levelObject)
        {
        }

        public override void Construct(BitWriter writer)
        {
            writer.WriteBit(false);
            writer.Write(0);

            Serialize(writer);
        }

        public override void Serialize(BitWriter writer)
        {
            writer.WriteBit(false);
            writer.WriteBit(false);

            writer.WriteBit(HasPosition);

            if (!HasPosition) return;

            writer.Write(Transform.Position);
            writer.Write(Transform.Rotation);
        }
    }
}