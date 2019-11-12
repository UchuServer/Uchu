using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Uchu.Core.Client
{
	[Table("MotionFX")]
	public class MotionFX
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[Column("id")]
		public int? Id { get; set; }

		[Column("typeID")]
		public int? TypeID { get; set; }

		[Column("slamVelocity")]
		public float? SlamVelocity { get; set; }

		[Column("addVelocity")]
		public float? AddVelocity { get; set; }

		[Column("duration")]
		public float? Duration { get; set; }

		[Column("destGroupName")]
		public string DestGroupName { get; set; }

		[Column("startScale")]
		public float? StartScale { get; set; }

		[Column("endScale")]
		public float? EndScale { get; set; }

		[Column("velocity")]
		public float? Velocity { get; set; }

		[Column("distance")]
		public float? Distance { get; set; }
	}
}