using RakDotNet;
using Uchu.Core;

namespace Uchu.World
{
    public class PickupItemMessage : ClientGameMessage
    {
        public override ushort GameMessageId => 0x008B;

        public long LootObjectId { get; set; }
        public long PlayerObjectId { get; set; }

        public override void Deserialize(BitStream stream)
        {
            LootObjectId = stream.ReadLong();
            PlayerObjectId = stream.ReadLong();
        }
    }
}