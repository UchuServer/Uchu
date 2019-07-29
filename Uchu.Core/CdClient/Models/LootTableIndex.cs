using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Uchu.Core.CdClient
{
	[Table("LootTableIndex")]
	public class LootTableIndex
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[Column("LootTableIndex")]
		public int? LootTableIndexColumn { get; set; }
	}
}