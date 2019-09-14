using RakDotNet.IO;

namespace Uchu.World
{
    public class EquipItemMessage : ClientGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.EquipInventory;
        
        public bool IgnoreCooldown { get; set; }
        
        public bool OutSuccess { get; set; }
        
        public Item Item { get; set; }

        public override void Deserialize(BitReader reader)
        {
            IgnoreCooldown = reader.ReadBit();
            OutSuccess = reader.ReadBit();

            Item = reader.ReadGameObject<Item>(Associate.Zone);
        }
    }
}