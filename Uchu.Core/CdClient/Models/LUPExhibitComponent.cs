using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Uchu.Core.CdClient
{
	[Table("LUPExhibitComponent")]
	public class LUPExhibitComponent
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[Column("id")]
		public int? Id { get; set; }

		[Column("minXZ")]
		public float? MinXZ { get; set; }

		[Column("maxXZ")]
		public float? MaxXZ { get; set; }

		[Column("maxY")]
		public float? MaxY { get; set; }

		[Column("offsetX")]
		public float? OffsetX { get; set; }

		[Column("offsetY")]
		public float? OffsetY { get; set; }

		[Column("offsetZ")]
		public float? OffsetZ { get; set; }
	}
}