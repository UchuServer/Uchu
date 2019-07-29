using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Uchu.Core.CdClient
{
	[Table("CurrencyDenominations")]
	public class CurrencyDenominations
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[Column("value")]
		public int? Value { get; set; }

		[Column("objectid")]
		public int? Objectid { get; set; }
	}
}