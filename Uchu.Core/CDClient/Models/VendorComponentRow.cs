using System.ComponentModel.DataAnnotations.Schema;

namespace Uchu.Core
{
    public class VendorComponentRow
    {
        [Column("id")]
        public int Id { get; set; }
        
        [Column("buyScalar")]
        public float BuyScale { get; set; }
        
        [Column("sellScalar")]
        public float SellScale { get; set; }
        
        [Column("refreshTimeInSeconds")]
        public int RefreshTime { get; set; }
        
        [Column("LootMatrixIndex")]
        public int LootMatrixIndex { get; set; }
    }
}