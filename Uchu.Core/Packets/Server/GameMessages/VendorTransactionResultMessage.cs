using RakDotNet;

namespace Uchu.Core
{
    public class VendorTransactionResultMessage : ServerGameMessage
    {
        public override ushort GameMessageId => 0x01dc;

        public TransactionResult TransactionResult { get; set; }
        
        public override void SerializeMessage(BitStream stream)
        {
            stream.WriteInt32((int) TransactionResult);
        }
    }
}