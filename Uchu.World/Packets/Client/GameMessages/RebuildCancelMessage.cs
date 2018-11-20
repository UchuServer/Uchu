using RakDotNet;
using Uchu.Core;

namespace Uchu.World
{
    public class RebuildCancelMessage : ClientGameMessage
    {
        public override ushort GameMessageId => 0x00D1;

        public bool IsReleasedEarly { get; set; }
        public long PlayerObjectId { get; set; }

        public override void Deserialize(BitStream stream)
        {
            IsReleasedEarly = stream.ReadBit();
            PlayerObjectId = stream.ReadLong();
        }
    }
}