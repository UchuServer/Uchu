using RakDotNet.IO;
using Uchu.World.Parsers;

namespace Uchu.World
{
    [Essential]
    public class VehiclePhysicsComponent : ReplicaComponent
    {
        public override ReplicaComponentsId Id => ReplicaComponentsId.VehiclePhysics;

        public override void FromLevelObject(LevelObject levelObject)
        {
            
        }

        public override void Construct(BitWriter writer)
        {
            writer.Write<byte>(0);
            writer.WriteBit(false);

            Serialize(writer);
        }

        public override void Serialize(BitWriter writer)
        {
            writer.WriteBit(false);
        }
    }
}