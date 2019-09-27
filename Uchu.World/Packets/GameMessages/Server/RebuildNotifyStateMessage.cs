using RakDotNet.IO;

namespace Uchu.World
{
    public class RebuildNotifyStateMessage : ServerGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.RebuildNotifyState;

        public RebuildState CurrentState { get; set; }
        
        public RebuildState NewState { get; set; }

        public Player Player;
        
        public override void SerializeMessage(BitWriter writer)
        {
            writer.Write((int) CurrentState);
            writer.Write((int) NewState);

            writer.Write(Player);
        }
    }
}