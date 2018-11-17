using RakDotNet;
using Uchu.Core;

namespace Uchu.World
{
    public class EquipItemRequestMessage : ClientGameMessage
    {
        public override ushort GameMessageId => 0x00E7;

        public bool IgnoreCooldown { get; set; }
        public bool OutSuccess { get; set; }
        public long ItemObjectId { get; set; }

        public override void Deserialize(BitStream stream)
        {
            IgnoreCooldown = stream.ReadBit();
            OutSuccess = stream.ReadBit();
            ItemObjectId = stream.ReadLong();
        }
    }
}