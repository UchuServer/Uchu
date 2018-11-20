using RakDotNet;
using Uchu.Core;

namespace Uchu.World
{
    public class EnableRebuildMessage : ServerGameMessage
    {
        public override ushort GameMessageId => 0x00D5;

        public bool Enable { get; set; } = false;
        public bool IsFail { get; set; } = false;
        public bool IsSuccess { get; set; } = false;
        public RebuildFailReason FailReason { get; set; } = RebuildFailReason.Unknown;
        public float Duration { get; set; } = 0;
        public long PlayerObjectId { get; set; }

        public override void SerializeMessage(BitStream stream)
        {
            stream.WriteBit(Enable);
            stream.WriteBit(IsFail);
            stream.WriteBit(IsSuccess);

            var hasReason = FailReason != RebuildFailReason.Unknown;

            stream.WriteBit(hasReason);

            if (hasReason)
                stream.WriteUInt((uint) FailReason);

            stream.WriteFloat(Duration);
            stream.WriteLong(PlayerObjectId);
        }
    }
}