using RakDotNet;

namespace Uchu.Core
{
    public class EquipItemMessage : ServerGameMessage
    {
        public override ushort GameMessageId => 0x00E7;

        public bool IgnoreCooldown { get; set; } = false;
        public bool OutSuccess { get; set; } = false;
        public long ItemObjectId { get; set; }

        public override void SerializeMessage(BitStream stream)
        {
            stream.WriteBit(IgnoreCooldown);
            stream.WriteBit(OutSuccess);
            stream.WriteLong(ItemObjectId);
        }
    }
}