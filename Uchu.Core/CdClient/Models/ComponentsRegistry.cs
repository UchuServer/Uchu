using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Uchu.Core.CdClient
{
	[Table("ComponentsRegistry")]
	public class ComponentsRegistry
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[Column("id")]
		public int? Id { get; set; }

		[Column("component_type")]
		public int? Componenttype { get; set; }

		[Column("component_id")]
		public int? Componentid { get; set; }
	}
}