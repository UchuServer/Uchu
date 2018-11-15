using RakDotNet;
using Uchu.Core;

namespace Uchu.World
{
    public class SetFlagMessage : ClientGameMessage
    {
        public override ushort GameMessageId => 0x01D7;

        public bool Flag { get; set; }
        public int FlagId { get; set; }

        public override void Deserialize(BitStream stream)
        {
            Flag = stream.ReadBit();
            FlagId = stream.ReadInt();
        }
    }
}