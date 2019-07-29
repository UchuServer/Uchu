using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Uchu.Core.CdClient
{
	[Table("SmashableChain")]
	public class SmashableChain
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[Column("chainIndex")]
		public int? ChainIndex { get; set; }

		[Column("chainLevel")]
		public int? ChainLevel { get; set; }

		[Column("lootMatrixID")]
		public int? LootMatrixID { get; set; }

		[Column("rarityTableIndex")]
		public int? RarityTableIndex { get; set; }

		[Column("currencyIndex")]
		public int? CurrencyIndex { get; set; }

		[Column("currencyLevel")]
		public int? CurrencyLevel { get; set; }

		[Column("smashCount")]
		public int? SmashCount { get; set; }

		[Column("timeLimit")]
		public int? TimeLimit { get; set; }

		[Column("chainStepID")]
		public int? ChainStepID { get; set; }
	}
}