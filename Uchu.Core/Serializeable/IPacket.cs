using RakDotNet;

namespace Uchu.Core
{
    public interface IPacket : ISerializable
    {
        /// <summary>
        ///     Remote connection type
        /// </summary>
        RemoteConnectionType RemoteConnectionType { get; }
        
        /// <summary>
        ///     Packet identifier
        /// </summary>
        uint PacketId { get; }
    }
}