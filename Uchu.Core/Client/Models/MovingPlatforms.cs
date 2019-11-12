using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Uchu.Core.Client
{
	[Table("MovingPlatforms")]
	public class MovingPlatforms
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[Column("id")]
		public int? Id { get; set; }

		[Column("platformIsSimpleMover")]
		public bool? PlatformIsSimpleMover { get; set; }

		[Column("platformMoveX")]
		public float? PlatformMoveX { get; set; }

		[Column("platformMoveY")]
		public float? PlatformMoveY { get; set; }

		[Column("platformMoveZ")]
		public float? PlatformMoveZ { get; set; }

		[Column("platformMoveTime")]
		public float? PlatformMoveTime { get; set; }

		[Column("platformStartAtEnd")]
		public bool? PlatformStartAtEnd { get; set; }

		[Column("description")]
		public string Description { get; set; }
	}
}