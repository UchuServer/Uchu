using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.World
{
    public class MarkInventoryItemAsActiveMessage : ServerGameMessage
    {
        public override GameMessageId GameMessageId { get; } = GameMessageId.MarkInventoryItemAsActive;
        public bool bActive = false;
        public int iType = 0;
        public ObjectId itemID = (ObjectId)(ulong)0;
        public override void SerializeMessage(BitWriter writer)
        {
            writer.WriteBit(bActive);

            writer.WriteBit(iType != 0);
            if (iType != 0) writer.Write(iType);
            
            writer.WriteBit(itemID != (ObjectId)(ulong)0);
            if (itemID != (ObjectId)(ulong)0) writer.Write((ulong)itemID);
        }
    }
}