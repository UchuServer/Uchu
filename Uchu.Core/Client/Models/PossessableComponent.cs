using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Uchu.Core.Client
{
	[Table("PossessableComponent")]
	public class PossessableComponent
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[Column("id")]
		public int? Id { get; set; }

		[Column("controlSchemeID")]
		public int? ControlSchemeID { get; set; }

		[Column("minifigAttachPoint")]
		public string MinifigAttachPoint { get; set; }

		[Column("minifigAttachAnimation")]
		public string MinifigAttachAnimation { get; set; }

		[Column("minifigDetachAnimation")]
		public string MinifigDetachAnimation { get; set; }

		[Column("mountAttachAnimation")]
		public string MountAttachAnimation { get; set; }

		[Column("mountDetachAnimation")]
		public string MountDetachAnimation { get; set; }

		[Column("attachOffsetFwd")]
		public float? AttachOffsetFwd { get; set; }

		[Column("attachOffsetRight")]
		public float? AttachOffsetRight { get; set; }

		[Column("possessionType")]
		public int? PossessionType { get; set; }

		[Column("wantBillboard")]
		public bool? WantBillboard { get; set; }

		[Column("billboardOffsetUp")]
		public float? BillboardOffsetUp { get; set; }

		[Column("depossessOnHit")]
		public bool? DepossessOnHit { get; set; }

		[Column("hitStunTime")]
		public float? HitStunTime { get; set; }

		[Column("skillSet")]
		public int? SkillSet { get; set; }
	}
}