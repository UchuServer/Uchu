using RakDotNet.IO;
using Uchu.World.Parsers;

namespace Uchu.World
{
    public class PossesableComponent : ReplicaComponent
    {
        public GameObject Driver { get; set; }
        
        public override ReplicaComponentsId Id => ReplicaComponentsId.Possesable;

        public override void FromLevelObject(LevelObject levelObject)
        {
            
        }

        public override void Construct(BitWriter writer)
        {
            Serialize(writer);
        }

        public override void Serialize(BitWriter writer)
        {
            writer.Write(true);

            var hasDriver = Driver != null;

            writer.WriteBit(hasDriver);

            if (hasDriver) writer.Write(Driver);

            writer.WriteBit(false);
            writer.WriteBit(false);
        }
    }
}