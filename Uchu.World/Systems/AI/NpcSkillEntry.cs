namespace Uchu.World.Systems.AI
{
    /// <summary>
    /// A skill entry in the NPC skills table, used to track cooldowns
    /// </summary>
    public class NpcSkillEntry
    {
        /// <summary>
        /// The id of the skill
        /// </summary>
        public uint SkillId { get; set; }
        
        /// <summary>
        /// The current cooldown time
        /// </summary>
        public float Cooldown { get; set; }

        /// <summary>
        /// The cooldown time in milliseconds that should be set after activating this ability
        /// </summary>
        public float AbilityCooldown { get; set; }
    }
}