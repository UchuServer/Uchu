using RakDotNet.IO;

namespace Uchu.World
{
    public class PlayerReadyMessage : ServerGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.PlayerReady;

        public override void SerializeMessage(BitWriter writer)
        {
        }
    }
}