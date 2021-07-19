using RakDotNet;
using Uchu.Core;

namespace Uchu.World
{
    public class ClientGameMessagePacketStructAttribute : WorldPacketStructAttribute
    {
        /// <summary>
        /// Creates the packet struct attribute for a Server Game Message.
        /// </summary>
        /// <param name="remoteConnectionType">Remote connection type of the packet.</param>
        /// <param name="packetId">Id of the packet.</param>
        public ClientGameMessagePacketStructAttribute() : base(MessageIdentifier.UserPacketEnum, RemoteConnectionType.Client, 0x5)
        {
            
        }
    }
}