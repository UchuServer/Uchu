using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Uchu.Core.CdClient
{
	[Table("RacingModuleComponent")]
	public class RacingModuleComponent
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[Column("id")]
		public int? Id { get; set; }

		[Column("topSpeed")]
		public float? TopSpeed { get; set; }

		[Column("acceleration")]
		public float? Acceleration { get; set; }

		[Column("handling")]
		public float? Handling { get; set; }

		[Column("stability")]
		public float? Stability { get; set; }

		[Column("imagination")]
		public float? Imagination { get; set; }
	}
}