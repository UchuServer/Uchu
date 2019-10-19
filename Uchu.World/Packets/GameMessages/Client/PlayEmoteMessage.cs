using RakDotNet.IO;

namespace Uchu.World
{
    public class PlayEmoteMessage : ClientGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.PlayEmote;

        public int EmoteId { get; set; }
        
        public GameObject Target { get; set; }
        
        public override void Deserialize(BitReader reader)
        {
            EmoteId = reader.Read<int>();

            Target = reader.ReadGameObject(Associate.Zone);
        }
    }
}