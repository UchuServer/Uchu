using RakDotNet.IO;

namespace Uchu.World
{
    public class RespondToMissionMessage : ClientGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.RespondToMission;

        public int MissionId { get; set; }

        public Player Player { get; set; }

        public GameObject Receiver { get; set; }

        public Lot RewardItem { get; set; }

        public override void Deserialize(BitReader reader)
        {
            MissionId = reader.Read<int>();

            Player = (Player) reader.ReadGameObject(Associate.Zone);

            Receiver = reader.ReadGameObject(Associate.Zone);

            RewardItem = reader.ReadBit() ? reader.Read<int>() : -1;
        }
    }
}