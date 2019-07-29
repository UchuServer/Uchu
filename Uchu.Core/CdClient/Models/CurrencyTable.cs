using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Uchu.Core.CdClient
{
	[Table("CurrencyTable")]
	public class CurrencyTable
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[Column("currencyIndex")]
		public int? CurrencyIndex { get; set; }

		[Column("npcminlevel")]
		public int? Npcminlevel { get; set; }

		[Column("minvalue")]
		public int? Minvalue { get; set; }

		[Column("maxvalue")]
		public int? Maxvalue { get; set; }

		[Column("id")]
		public int? Id { get; set; }
	}
}