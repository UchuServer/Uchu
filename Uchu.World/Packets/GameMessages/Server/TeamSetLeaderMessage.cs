using RakDotNet.IO;

namespace Uchu.World
{
    public class TeamSetLeaderMessage : ServerGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.TeamSetLeader;
        
        public Player NewLeader { get; set; }
        
        public override void SerializeMessage(BitWriter writer)
        {
            writer.Write(NewLeader.ObjectId);
        }
    }
}