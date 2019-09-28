using System.Numerics;
using RakDotNet.IO;
using Uchu.World.Parsers;

namespace Uchu.World
{
    public class BouncerComponent : ReplicaComponent
    {
        public bool PetRequired { get; set; }
        
        public Vector3 BouncerDestination { get; set; }

        public override ReplicaComponentsId Id => ReplicaComponentsId.Bouncer;

        public override void FromLevelObject(LevelObject levelObject)
        {
            BouncerDestination = (Vector3) levelObject.Settings["bouncer_destination"];
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