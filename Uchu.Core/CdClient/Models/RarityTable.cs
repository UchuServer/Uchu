using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Uchu.Core.CdClient
{
	[Table("RarityTable")]
	public class RarityTable
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[Column("id")]
		public int? Id { get; set; }

		[Column("randmax")]
		public float? Randmax { get; set; }

		[Column("rarity")]
		public int? Rarity { get; set; }

		[Column("RarityTableIndex")]
		public int? RarityTableIndex { get; set; }
	}
}