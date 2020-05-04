using RakDotNet.IO;

namespace Uchu.World
{
    public class UseNonEquipmentItemMessage : ClientGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.UseNonEquipmentItem;
        
        public Item Item { get; set; }

        public override void Deserialize(BitReader reader)
        {
            Item = reader.ReadGameObject<Item>(Associate.Zone);
        }
    }
}