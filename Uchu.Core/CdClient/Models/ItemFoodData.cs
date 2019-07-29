using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Uchu.Core.CdClient
{
	[Table("ItemFoodData")]
	public class ItemFoodData
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[Column("id")]
		public int? Id { get; set; }

		[Column("element_1")]
		public int? Element1 { get; set; }

		[Column("element_1_amount")]
		public int? Element1amount { get; set; }

		[Column("element_2")]
		public int? Element2 { get; set; }

		[Column("element_2_amount")]
		public int? Element2amount { get; set; }

		[Column("element_3")]
		public int? Element3 { get; set; }

		[Column("element_3_amount")]
		public int? Element3amount { get; set; }

		[Column("element_4")]
		public int? Element4 { get; set; }

		[Column("element_4_amount")]
		public int? Element4amount { get; set; }
	}
}