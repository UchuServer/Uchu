using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.World
{
    public class UiMessageServerToSingleClientMessage : ServerGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.UIMessageServerToSingleClient;
        
        public byte[] Content { get; set; }
        
        public string MessageName { get; set; }
        
        public override void SerializeMessage(BitWriter writer)
        {
            foreach (var value in Content)
            {
                writer.Write(value);
            }

            writer.Write((uint) MessageName.Length);
            
            writer.WriteString(MessageName, MessageName.Length);
        }
    }
}