using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.World
{
    public class MatchUpdate : ServerGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.MatchUpdate;
        
        public string Data { get; set; }
        
        public MatchUpdateType Type { get; set; }
        
        public override void SerializeMessage(BitWriter writer)
        {
            writer.Write((uint) Data.Length);

            if (Data.Length > 0)
            {
                writer.WriteString(Data, Data.Length, true);
                writer.Write((byte) 0);
                writer.Write((byte) 0);
            }

            writer.Write((int) Type);
        }
    }
}