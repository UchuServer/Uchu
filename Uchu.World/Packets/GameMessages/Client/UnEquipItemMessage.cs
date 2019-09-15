using RakDotNet.IO;

namespace Uchu.World
{
    public class UnEquipItemMessage : ClientGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.UnEquipInventory;

        public bool EvenIfDead { get; set; }

        public bool IgnoreCooldown { get; set; }

        public bool OutSuccess { get; set; }

        public Item ItemToUnEquip { get; set; }

        public Item ReplacementItem { get; set; }

        public override void Deserialize(BitReader reader)
        {
            EvenIfDead = reader.ReadBit();
            IgnoreCooldown = reader.ReadBit();
            OutSuccess = reader.ReadBit();

            ItemToUnEquip = reader.ReadGameObject<Item>(Associate.Zone);
            ReplacementItem = reader.ReadGameObject<Item>(Associate.Zone);
        }
    }
}