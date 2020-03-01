namespace Uchu.World
{
    public struct SkillEntry
    {
        public uint SkillId { get; set; }
        
        public SkillCastType Type { get; set; }
        
        public int AiCombatWeight { get; set; }
    }
}