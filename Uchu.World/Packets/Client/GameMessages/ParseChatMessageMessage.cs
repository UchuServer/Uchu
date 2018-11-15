using RakDotNet;
using Uchu.Core;

namespace Uchu.World
{
    public class ParseChatMessageMessage : ClientGameMessage
    {
        public override ushort GameMessageId => 0x0352;

        public int ClientState { get; set; }
        public string Message { get; set; }

        public override void Deserialize(BitStream stream)
        {
            ClientState = stream.ReadInt();
            Message = stream.ReadString((int) stream.ReadUInt(), true);
        }
    }
}