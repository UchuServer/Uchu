using RakDotNet.IO;

namespace Uchu.World
{
    public class SetFlagMessage : ClientGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.SetFlag;

        public bool Flag { get; set; }
        
        public int FlagId { get; set; }
        
        public override void Deserialize(BitReader reader)
        {
            Flag = reader.ReadBit();
            FlagId = reader.Read<int>();
        }
    }
}