using RakDotNet.IO;

namespace Uchu.World
{
    public class SelectSkillMessage : ClientGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.SelectSkill;
        
        public bool FromSkillSet { get; set; }
        
        public int SkillId { get; set; }

        public override void Deserialize(BitReader reader)
        {
            FromSkillSet = reader.ReadBit();

            SkillId = reader.Read<int>();
        }
    }
}