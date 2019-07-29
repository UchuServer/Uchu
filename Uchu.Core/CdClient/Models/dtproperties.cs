using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Uchu.Core.CdClient
{
	[Table("dtproperties")]
	public class dtproperties
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[Column("id")]
		public int? Id { get; set; }

		[Column("objectid")]
		public int? Objectid { get; set; }

		[Column("property")]
		public string Property { get; set; }

		[Column("value")]
		public string Value { get; set; }

		[Column("uvalue")]
		public string Uvalue { get; set; }

		[Column("lvalue")]
		public int? Lvalue { get; set; }

		[Column("version")]
		public int? Version { get; set; }
	}
}