using System.ComponentModel.DataAnnotations.Schema;

namespace Uchu.Core
{
    public class DestructibleComponentRow
    {
        [Column("id")]
        public int DestructibleId { get; set; }

        [Column("factionList")]
        public int[] Factions { get; set; }

        [Column("life")]
        public int Health { get; set; }

        [Column("imagination")]
        public int Imagination { get; set; }

        [Column("LootMatrixIndex")]
        public int LootMatrixIndex { get; set; }

        [Column("CurrencyIndex")]
        public int CurrencyIndex { get; set; }

        [Column("level")]
        public int Level { get; set; }

        [Column("armor")]
        public float Armor { get; set; }

        [Column("death_behavior")]
        public int DeathBehavior { get; set; }

        [Column("isnpc")]
        public bool IsNPC { get; set; }

        [Column("attack_priority")]
        public int AttackPriority { get; set; }

        [Column("isSmashable")]
        public bool IsSmashable { get; set; }

        [Column("difficultyLevel")]
        public int DifficultyLevel { get; set; }
    }
}