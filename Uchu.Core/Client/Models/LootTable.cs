using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Uchu.Core.Client
{
	[Table("LootTable")]
	public class LootTable
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[Column("itemid")]
		public int? Itemid { get; set; }

		[Column("LootTableIndex")]
		public int? LootTableIndex { get; set; }

		[Column("id")]
		public int? Id { get; set; }

		[Column("MissionDrop")]
		public bool? MissionDrop { get; set; }

		[Column("sortPriority")]
		public int? SortPriority { get; set; }
	}
}