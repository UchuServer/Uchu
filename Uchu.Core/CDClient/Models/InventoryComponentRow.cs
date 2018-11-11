using System.ComponentModel.DataAnnotations.Schema;

namespace Uchu.Core
{
    public class InventoryComponentRow
    {
        [Column("id")]
        public int InventoryId { get; set; }

        [Column("itemid")]
        public int ItemId { get; set; }

        [Column("count")]
        public int ItemCount { get; set; }

        [Column("equip")]
        public bool Equipped { get; set; }
    }
}