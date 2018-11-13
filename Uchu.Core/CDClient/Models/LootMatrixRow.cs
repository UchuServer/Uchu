using System.ComponentModel.DataAnnotations.Schema;

namespace Uchu.Core
{
    public class LootMatrixRow
    {
        [Column("LootMatrixIndex")]
        public int LootMatrixIndex { get; set; }

        [Column("LootTableIndex")]
        public int LootTableIndex { get; set; }

        [Column("RarityTableIndex")]
        public int RarityTableIndex { get; set; }

        [Column("percent")]
        public float Percent { get; set; }

        [Column("minToDrop")]
        public int MinDrops { get; set; }

        [Column("maxToDrop")]
        public int MaxDrops { get; set; }

        [Column("id")]
        public int Id { get; set; }

        [Column("flagID")]
        public int FlagId { get; set; }

        [Column("gate_version")]
        public string GateVersion { get; set; }
    }
}