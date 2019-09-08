using Uchu.Core;

namespace Uchu.World
{
    public interface IGameMessage : IPacket
    {
        GameMessageId GameMessageId { get; }

        GameObject Associate { get; set; }
    }
}