namespace Uchu.World
{
    public class PossessableOccupantComponent : StructReplicaComponent<PossessableOccupantSerialization>
    {
        public override ComponentId Id => ComponentId.Component107;

        public GameObject Possessed { get; set; }
    }
}