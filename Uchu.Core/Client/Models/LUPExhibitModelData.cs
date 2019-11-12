using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Uchu.Core.Client
{
	[Table("LUPExhibitModelData")]
	public class LUPExhibitModelData
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[Column("LOT")]
		public int? LOT { get; set; }

		[Column("minXZ")]
		public float? MinXZ { get; set; }

		[Column("maxXZ")]
		public float? MaxXZ { get; set; }

		[Column("maxY")]
		public float? MaxY { get; set; }

		[Column("description")]
		public string Description { get; set; }

		[Column("owner")]
		public string Owner { get; set; }
	}
}