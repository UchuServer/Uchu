using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Uchu.Core.Client
{
	[Table("AccessoryDefaultLoc")]
	public class AccessoryDefaultLoc
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[Column("GroupID")]
		public int? GroupID { get; set; }

		[Column("Description")]
		public string Description { get; set; }

		[Column("Pos_X")]
		public float? PosX { get; set; }

		[Column("Pos_Y")]
		public float? PosY { get; set; }

		[Column("Pos_Z")]
		public float? PosZ { get; set; }

		[Column("Rot_X")]
		public float? RotX { get; set; }

		[Column("Rot_Y")]
		public float? RotY { get; set; }

		[Column("Rot_Z")]
		public float? RotZ { get; set; }
	}
}