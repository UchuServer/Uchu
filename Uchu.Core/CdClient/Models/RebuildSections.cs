using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Uchu.Core.CdClient
{
	[Table("RebuildSections")]
	public class RebuildSections
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[Column("id")]
		public int? Id { get; set; }

		[Column("rebuildID")]
		public int? RebuildID { get; set; }

		[Column("objectID")]
		public int? ObjectID { get; set; }

		[Column("offset_x")]
		public float? Offsetx { get; set; }

		[Column("offset_y")]
		public float? Offsety { get; set; }

		[Column("offset_z")]
		public float? Offsetz { get; set; }

		[Column("fall_angle_x")]
		public float? Fallanglex { get; set; }

		[Column("fall_angle_y")]
		public float? Fallangley { get; set; }

		[Column("fall_angle_z")]
		public float? Fallanglez { get; set; }

		[Column("fall_height")]
		public float? Fallheight { get; set; }

		[Column("requires_list")]
		public string Requireslist { get; set; }

		[Column("size")]
		public int? Size { get; set; }

		[Column("bPlaced")]
		public bool? BPlaced { get; set; }
	}
}