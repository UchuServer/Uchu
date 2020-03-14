using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Uchu.Core
{
    public class CharacterTrade
    {
        [Key]
        public long Id { get; set; }
        
        public long PartyA { get; set; }
        
        public long PartyB { get; set; }
        
        public long CurrencyA { get; set; }
        
        public long CurrencyB { get; set; }
        
        public List<TradeTransactionItem> TransactionItems { get; set; }
    }
}