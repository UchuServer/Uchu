using RakDotNet.IO;
using Uchu.World.Parsers;

namespace Uchu.World
{
    [Essential]
    public class BouncerComponent : ReplicaComponent
    {
        public bool PetRequired { get; set; } = false;
        
        public override ReplicaComponentsId Id => ReplicaComponentsId.Bouncer;

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
            writer.WriteBit(!PetRequired);
        }
    }
}