using RakDotNet;
using Uchu.Core;

namespace Uchu.World
{
    public class HasBeenCollectedMessage : ClientGameMessage
    {
        public override ushort GameMessageId => 0x01E6;

        public long PlayerObjectId { get; set; }

        public override void Deserialize(BitStream stream)
        {
            PlayerObjectId = stream.ReadLong();
        }
    }
}