using RakDotNet.IO;

namespace Uchu.World
{
    public class TeamSetLeaderMessage : ServerGameMessage
    {
        public override ushort GameMessageId => 0x615;
        
        public Player NewLeader { get; set; }
        
        public override void SerializeMessage(BitWriter writer)
        {
            writer.Write(NewLeader.ObjectId);
        }
    }
}