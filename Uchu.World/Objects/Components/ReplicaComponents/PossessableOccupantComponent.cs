using RakDotNet.IO;

namespace Uchu.World
{
    public class PossessableOccupantComponent : ReplicaComponent
    {
        public override ComponentId Id => ComponentId.Component107;

        public GameObject Possessed { get; set; }
        
        public override void Construct(BitWriter writer)
        {
            Serialize(writer);
        }

        public override void Serialize(BitWriter writer)
        {
            if (writer.Flag(Possessed != default))
                writer.Write(Possessed);
        }
    }
}