using RakDotNet.IO;

namespace Uchu.World
{
    public class NotifyTamingModelLoadedOnServerMessage : ServerGameMessage
    {
        public override GameMessageId GameMessageId { get; } = GameMessageId.NotifyTamingModelLoadedOnServer;
        public override void SerializeMessage(BitWriter writer)
        {
            
        }
    }
}