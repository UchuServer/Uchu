using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Uchu.Core.Client
{
	[Table("mapShaders")]
	public class mapShaders
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[Column("id")]
		public int? Id { get; set; }

		[Column("label")]
		public string Label { get; set; }

		[Column("gameValue")]
		public int? GameValue { get; set; }

		[Column("priority")]
		public int? Priority { get; set; }
	}
}