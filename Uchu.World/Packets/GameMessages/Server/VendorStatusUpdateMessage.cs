using RakDotNet.IO;

namespace Uchu.World
{
    public class VendorStatusUpdateMessage : ServerGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.VendorStatusUpdate;
        
        public bool UpdateOnly { get; set; }
        
        public ShopEntry[] Entries { get; set; }
        
        public override void SerializeMessage(BitWriter writer)
        {
            writer.WriteBit(UpdateOnly);

            writer.Write((uint) Entries.Length);

            foreach (var entry in Entries)
            {
                writer.Write(entry.Lot);
                writer.Write(entry.SortPriority);
            }
        }
    }
}