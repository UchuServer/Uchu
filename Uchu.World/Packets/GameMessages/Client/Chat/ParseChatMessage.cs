using RakDotNet;
using RakDotNet.IO;

namespace Uchu.World.Chat
{
    public class ParseChatMessage : ClientGameMessage
    {
        public override ushort GameMessageId => 0x352;

        public int ClientState { get; set; }
        
        public string Message { get; set; }
        
        public override void Deserialize(BitReader reader)
        {
            ClientState = reader.Read<int>();
            Message = reader.ReadString((int) reader.Read<uint>(), true);
        }
    }
}