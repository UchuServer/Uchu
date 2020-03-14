using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Uchu.Core
{
    public class TradeTransactionItem
    {
        [Key]
        public long Id { get; set; }
        
        public long ItemId { get; set; }
        
        public long Party { get; set; }
        
        public long TradeId { get; set; }
        
        [ForeignKey(nameof(TradeId))]
        public CharacterTrade Trade { get; set; }
    }
}