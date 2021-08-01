using System;
using RakDotNet;

namespace Uchu.Core
{
    /// <summary>
    /// Header information about a packet.
    /// </summary>
    [AttributeUsage(AttributeTargets.Struct)]
    public class PacketStructAttribute : Attribute
    {
        /// <summary>
        /// Message identifier.
        /// </summary>
        public MessageIdentifier MessageIdentifier { get; }
        
        /// <summary>
        /// Remote connection type.
        /// </summary>
        public RemoteConnectionType RemoteConnectionType { get; }
        
        /// <summary>
        /// Packet identifier.
        /// </summary>
        public uint PacketId { get; }

        /// <summary>
        /// Creates the packet struct attribute.
        /// </summary>
        /// <param name="messageIdentifier">Message identifier of the packet.</param>
        /// <param name="remoteConnectionType">Remote connection type of the packet.</param>
        /// <param name="packetId">Id of the packet.</param>
        public PacketStructAttribute(MessageIdentifier messageIdentifier,RemoteConnectionType remoteConnectionType, uint packetId)
        {
            this.MessageIdentifier = messageIdentifier;
            this.RemoteConnectionType = remoteConnectionType;
            this.PacketId = packetId;
        }
    }
}