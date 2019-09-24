using RakDotNet.IO;

namespace Uchu.Core
{
    public interface IPacket : ISerializable, IDeserializable
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