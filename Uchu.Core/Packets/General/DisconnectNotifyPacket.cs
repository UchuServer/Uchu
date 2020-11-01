using RakDotNet.IO;

namespace Uchu.Core
{
    public class DisconnectNotifyPacket : Packet
    {
        public override RemoteConnectionType RemoteConnectionType => RemoteConnectionType.General;

        public override uint PacketId => 0x1;
        
        public DisconnectId DisconnectId { get; set; }

        public override void SerializePacket(BitWriter writer)
        {
            writer.Write((uint) DisconnectId);
        }

        public override void Deserialize(BitReader reader)
        {
            DisconnectId = (DisconnectId) reader.Read<uint>();
        }
    }
}