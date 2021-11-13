namespace Uchu.World
{
    public class PossessableComponent : StructReplicaComponent<PossessableSerialization>
    {
        public GameObject Driver { get; set; }

        public override ComponentId Id => ComponentId.Possesable;

        /// <summary>
        /// Creates the packet for the replica component.
        /// </summary>
        /// <returns>The packet for the replica component.</returns>
        public override PossessableSerialization GetPacket()
        {
            var packet = base.GetPacket();
            packet.PossessableInfo = new PossessableInfo
            {
                Driver = this.Driver,
                ImmediateDepossess = false,
                Temp1 = false,
            };
            return packet;
        }
    }
}
