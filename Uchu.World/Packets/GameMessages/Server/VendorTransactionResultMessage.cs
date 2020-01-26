using RakDotNet.IO;

namespace Uchu.World
{
    public class VendorTransactionResultMessage : ServerGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.VendorTransactionResult;
        
        public TransactionResult Result { get; set; }
        
        public override void SerializeMessage(BitWriter writer)
        {
            writer.Write((int) Result);
        }
    }
}