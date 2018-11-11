using RakDotNet;
using Uchu.Core;

namespace Uchu.World
{
    public class ModifyLegoScoreMessage : ServerGameMessage
    {
        public override ushort GameMessageId => 0x05B3;

        public long Score { get; set; }
        public int SourceType { get; set; } = -1;

        public override void SerializeMessage(BitStream stream)
        {
            stream.WriteLong(Score);

            var hasType = SourceType != -1;

            stream.WriteBit(hasType);

            if (hasType)
                stream.WriteInt(SourceType);
        }
    }
}