using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Uchu.Core.CdClient
{
	[Table("SubscriptionPricing")]
	public class SubscriptionPricing
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[Column("id")]
		public int? Id { get; set; }

		[Column("countryCode")]
		public string CountryCode { get; set; }

		[Column("monthlyFeeGold")]
		public string MonthlyFeeGold { get; set; }

		[Column("monthlyFeeSilver")]
		public string MonthlyFeeSilver { get; set; }

		[Column("monthlyFeeBronze")]
		public string MonthlyFeeBronze { get; set; }

		[Column("monetarySymbol")]
		public int? MonetarySymbol { get; set; }

		[Column("symbolIsAppended")]
		public bool? SymbolIsAppended { get; set; }
	}
}