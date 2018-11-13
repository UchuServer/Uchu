using System.ComponentModel.DataAnnotations.Schema;

namespace Uchu.Core
{
    public class LootTableRow
    {
        [Column("itemid")]
        public int ItemId { get; set; }

        [Column("LootTableIndex")]
        public int LootTableIndex { get; set; }

        [Column("id")]
        public int Id { get; set; }

        [Column("MissionDrop")]
        public bool IsMissionDrop { get; set; }

        [Column("sortPriority")]
        public int SortPriority { get; set; }
    }
}