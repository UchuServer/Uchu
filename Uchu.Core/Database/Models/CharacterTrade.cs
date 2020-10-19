using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Uchu.Core
{
    [SuppressMessage("ReSharper", "CA2227")]
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
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