namespace Uchu.Core
{
    public interface IGameMessage : IPacket
    {
        ushort GameMessageId { get; }

        long ObjectId { get; set; }
    }
}