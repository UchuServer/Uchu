using System.ComponentModel.DataAnnotations.Schema;

namespace Uchu.Core
{
    public class CurrencyTableRow
    {
        [Column("currencyIndex")]
        public int CurrencyIndex { get; set; }

        [Column("npcminlevel")]
        public int MinimumNPCLevel { get; set; }

        [Column("minvalue")]
        public int MinimumValue { get; set; }

        [Column("maxvalue")]
        public int MaximumValue { get; set; }

        [Column("id")]
        public int CurrencyId { get; set; }
    }
}