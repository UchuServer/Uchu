using RakDotNet.IO;

namespace Uchu.World
{
    public class RemoveSkillMessage : ServerGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.RemoveSkill;
        
        public bool FromSkillSet { get; set; }
        
        public uint SkillId { get; set; }
        
        public override void SerializeMessage(BitWriter writer)
        {
            writer.WriteBit(FromSkillSet);

            writer.Write(SkillId);
        }
    }
}