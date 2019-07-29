using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Uchu.Core.CdClient
{
	[Table("RailActivatorComponent")]
	public class RailActivatorComponent
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[Column("id")]
		public int? Id { get; set; }

		[Column("startAnim")]
		public string StartAnim { get; set; }

		[Column("loopAnim")]
		public string LoopAnim { get; set; }

		[Column("stopAnim")]
		public string StopAnim { get; set; }

		[Column("startSound")]
		public string StartSound { get; set; }

		[Column("loopSound")]
		public string LoopSound { get; set; }

		[Column("stopSound")]
		public string StopSound { get; set; }

		[Column("effectIDs")]
		public string EffectIDs { get; set; }

		[Column("preconditions")]
		public string Preconditions { get; set; }

		[Column("playerCollision")]
		public bool? PlayerCollision { get; set; }

		[Column("cameraLocked")]
		public bool? CameraLocked { get; set; }

		[Column("StartEffectID")]
		public string StartEffectID { get; set; }

		[Column("StopEffectID")]
		public string StopEffectID { get; set; }

		[Column("DamageImmune")]
		public bool? DamageImmune { get; set; }

		[Column("NoAggro")]
		public bool? NoAggro { get; set; }

		[Column("ShowNameBillboard")]
		public bool? ShowNameBillboard { get; set; }
	}
}