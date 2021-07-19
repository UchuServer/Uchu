using RakDotNet.IO;

namespace Uchu.World
{
    public class RequestActivitySummaryLeaderboardDataMessage : GeneralGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.RequestActivitySummaryLeaderboardData;
        
        public int GameId { get; set; }
        
        public int QueryType { get; set; }
        
        public int ResultsEnd { get; set; }
        
        public int ResultsStart { get; set; }

        public GameObject Target { get; set; }
        
        public bool Weekly { get; set; }
        
        public override void SerializeMessage(BitWriter writer)
        {
            if (writer.Flag(GameId != default))
                writer.Write(GameId);
            if (writer.Flag(QueryType != 1))
                writer.Write(QueryType);
            if (writer.Flag(ResultsEnd != 10))
                writer.Write(ResultsEnd);
            if (writer.Flag(ResultsStart != 0))
                writer.Write(ResultsStart);
            writer.Write(Target);
            writer.Write(Weekly);
        }
        
        public override void Deserialize(BitReader reader)
        {
            if (reader.ReadBit())
                GameId = reader.Read<int>();
            if (reader.ReadBit())
                QueryType = reader.Read<int>();
            if (reader.ReadBit())
                ResultsEnd = reader.Read<int>();
            if (reader.ReadBit())
                ResultsStart = reader.Read<int>();
            Target = reader.ReadGameObject(this.Associate.Zone);
            Weekly = reader.ReadBit();
        }
    }
}