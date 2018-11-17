using RakDotNet;
using Uchu.Core;

namespace Uchu.World
{
    public class FireServerEventMessage : ClientGameMessage
    {
        public override ushort GameMessageId => 0x0302;

        public string Arguments { get; set; }

        public int FirstParameter { get; set; } = -1;
        public int SecondParameter { get; set; } = -1;
        public int ThirdParameter { get; set; } = -1;

        public long SenderObjectId { get; set; }

        public override void Deserialize(BitStream stream)
        {
            Arguments = stream.ReadString((int) stream.ReadUInt(), true);

            if (stream.ReadBit())
                FirstParameter = stream.ReadInt();

            if (stream.ReadBit())
                SecondParameter = stream.ReadInt();

            if (stream.ReadBit())
                ThirdParameter = stream.ReadInt();

            SenderObjectId = stream.ReadLong();
        }
    }
}