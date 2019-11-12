using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Uchu.Core.Client
{
	[Table("map_BlueprintCategory")]
	public class mapBlueprintCategory
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[Column("id")]
		public int? Id { get; set; }

		[Column("description")]
		public string Description { get; set; }

		[Column("enabled")]
		public bool? Enabled { get; set; }
	}
}