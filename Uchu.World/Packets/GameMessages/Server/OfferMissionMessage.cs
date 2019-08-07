using RakDotNet.IO;

namespace Uchu.World
{
    public class OfferMissionMessage : ServerGameMessage
    {
        public override ushort GameMessageId => 0xF8;
        
        public int MissionId { get; set; }
        
        public GameObject QuestGiver { get; set; }

        public override void SerializeMessage(BitWriter writer)
        {
            writer.Write(MissionId);

            writer.Write(QuestGiver);
        }
    }
}