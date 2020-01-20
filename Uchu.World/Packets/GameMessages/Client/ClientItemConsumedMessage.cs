using RakDotNet.IO;

namespace Uchu.World
{
    public class ClientItemConsumedMessage : ClientGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.ClientItemConsumed;

        public Item Item { get; set; }
        
        public override void Deserialize(BitReader reader)
        {
            Item = reader.ReadGameObject<Item>(Associate.Zone);
        }
    }
}