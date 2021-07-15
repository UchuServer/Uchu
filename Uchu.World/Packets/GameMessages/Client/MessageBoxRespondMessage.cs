using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.World
{
    public class MessageBoxRespondMessage : ClientGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.MessageBoxRespond;
        
        public int Button { get; set; }
        
        public string Identifier { get; set; }
        
        public string UserData { get; set; }

        public override void Deserialize(BitReader reader)
        {
            Button = reader.Read<int>();
            Identifier = reader.ReadString((int) reader.Read<uint>(), true);
            UserData = reader.ReadString((int) reader.Read<uint>(), true);
        }
    }
}