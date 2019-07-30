using Uchu.Core;

namespace Uchu.World
{
    public interface IGameMessage : IPacket
    {
        ushort GameMessageId { get; }

        GameObject Associate { get; set; }
    }
}