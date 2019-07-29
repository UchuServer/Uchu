using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Uchu.Core.CdClient
{
	[Table("BuffDefinitions")]
	public class BuffDefinitions
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[Column("ID")]
		public int? ID { get; set; }

		[Column("Priority")]
		public float? Priority { get; set; }

		[Column("UIIcon")]
		public string UIIcon { get; set; }
	}
}