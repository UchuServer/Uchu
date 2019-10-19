using RakDotNet.IO;

namespace Uchu.World
{
    public class PlayEmoteMessage : GeneralGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.PlayEmote;

        public int EmoteId { get; set; }
        
        public GameObject Target { get; set; }
        
        public override void SerializeMessage(BitWriter writer)
        {
            writer.Write(EmoteId);

            writer.Write(Target);
        }

        public override void Deserialize(BitReader reader)
        {
            EmoteId = reader.Read<int>();

            Target = reader.ReadGameObject(Associate.Zone);
        }
    }
}