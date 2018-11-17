using RakDotNet;

namespace Uchu.Core
{
    public class FireClientEventMessage : ServerGameMessage
    {
        public override ushort GameMessageId => 0x04BD;

        public string Arguments { get; set; }
        public long TargetObjectId { get; set; }
        public long FirstParameter { get; set; } = 0;
        public int SecondParameter { get; set; } = -1;
        public long SenderObjectId { get; set; }

        public override void SerializeMessage(BitStream stream)
        {
            stream.WriteUInt((uint) Arguments.Length);
            stream.WriteString(Arguments, Arguments.Length, true);

            stream.WriteLong(TargetObjectId);

            var hasParam1 = FirstParameter != 0;

            stream.WriteBit(hasParam1);

            if (hasParam1)
                stream.WriteLong(FirstParameter);

            var hasParam2 = SecondParameter != -1;

            stream.WriteBit(hasParam2);

            if (hasParam2)
                stream.WriteInt(SecondParameter);

            stream.WriteLong(SenderObjectId);
        }
    }
}