using RakDotNet;

namespace Uchu.Core
{
    public interface IPacket : ISerializable
    {
        RemoteConnectionType RemoteConnectionType { get; }
        uint PacketId { get; }
    }
}