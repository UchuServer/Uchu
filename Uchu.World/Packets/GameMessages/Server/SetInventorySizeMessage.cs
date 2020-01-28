using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.World
{
    public class SetInventorySizeMessage : ServerGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.SetInventorySize;
        
        public InventoryType InventoryType { get; set; }
        
        public int Size { get; set; }
        
        public override void SerializeMessage(BitWriter writer)
        {
            writer.Write((int) InventoryType);
            writer.Write(Size);
        }
    }
}