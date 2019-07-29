using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Uchu.Core.CdClient
{
	[Table("sysdiagrams")]
	public class sysdiagrams
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[Column("name")]
		public string Name { get; set; }

		[Column("principal_id")]
		public int? Principalid { get; set; }

		[Column("diagram_id")]
		public int? Diagramid { get; set; }

		[Column("version")]
		public int? Version { get; set; }

		[Column("definition")]
		public string Definition { get; set; }
	}
}