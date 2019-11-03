using RakDotNet.IO;

namespace Uchu.World
{
    public class RequestLinkedMissionMessage : ClientGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.RequestLinkedMission;
        
        public Player Player { get; set; }
        
        public int MissionId { get; set; }
        
        public bool MissionOffered { get; set; }

        public override void Deserialize(BitReader reader)
        {
            Player = reader.ReadGameObject<Player>(Associate.Zone);

            MissionId = reader.Read<int>();

            MissionOffered = reader.ReadBit();
        }
    }
}