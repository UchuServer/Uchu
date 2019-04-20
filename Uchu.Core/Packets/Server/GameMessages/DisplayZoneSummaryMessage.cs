using RakDotNet;

namespace Uchu.Core.Packets.Server.GameMessages
{
    public class DisplayZoneSummaryMessage : ServerGameMessage
    {
        public bool IsPropertyMap { get; set; }
        
        public bool IsZoneStart { get; set; }
        
        public long Sender { get; set; }
        
        public override ushort GameMessageId => 0x0413;

        public override void SerializeMessage(BitStream stream)
        {
            stream.WriteBit(IsPropertyMap);
            stream.WriteBit(IsZoneStart);
            stream.WriteInt64(Sender);
        }
    }
}