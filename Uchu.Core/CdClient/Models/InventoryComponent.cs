using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Uchu.Core.CdClient
{
	[Table("InventoryComponent")]
	public class InventoryComponent
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[Column("id")]
		public int? Id { get; set; }

		[Column("itemid")]
		public int? Itemid { get; set; }

		[Column("count")]
		public int? Count { get; set; }

		[Column("equip")]
		public bool? Equip { get; set; }
	}
}