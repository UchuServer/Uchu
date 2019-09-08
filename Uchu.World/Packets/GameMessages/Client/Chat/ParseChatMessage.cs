using RakDotNet;
using RakDotNet.IO;

namespace Uchu.World
{
    public class ParseChatMessage : ClientGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.ParseChatMessage;

        public int ClientState { get; set; }
        
        public string Message { get; set; }
        
        public override void Deserialize(BitReader reader)
        {
            ClientState = reader.Read<int>();
            Message = reader.ReadString((int) reader.Read<uint>(), true);
        }
    }
}