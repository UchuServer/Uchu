using RakDotNet;
using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.World.Social
{
    public class NotifyTeamInvitePacket : Packet
    {
        public override RemoteConnectionType RemoteConnectionType => RemoteConnectionType.Server;

        public override uint PacketId => 0x23;

        public GameObject Sender { get; set; }

        public override void SerializePacket(BitWriter writer)
        {
            writer.WriteString(Sender.Name, wide: true);

            writer.Write(Sender.ObjectId);
        }
    }
}