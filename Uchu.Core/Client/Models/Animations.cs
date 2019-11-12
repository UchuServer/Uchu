using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Uchu.Core.Client
{
	[Table("Animations")]
	public class Animations
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[Column("animationGroupID")]
		public int? AnimationGroupID { get; set; }

		[Column("animation_type")]
		public string Animationtype { get; set; }

		[Column("animation_name")]
		public string Animationname { get; set; }

		[Column("chance_to_play")]
		public float? Chancetoplay { get; set; }

		[Column("min_loops")]
		public int? Minloops { get; set; }

		[Column("max_loops")]
		public int? Maxloops { get; set; }

		[Column("animation_length")]
		public float? Animationlength { get; set; }

		[Column("hideEquip")]
		public bool? HideEquip { get; set; }

		[Column("ignoreUpperBody")]
		public bool? IgnoreUpperBody { get; set; }

		[Column("restartable")]
		public bool? Restartable { get; set; }

		[Column("face_animation_name")]
		public string Faceanimationname { get; set; }

		[Column("priority")]
		public float? Priority { get; set; }

		[Column("blendTime")]
		public float? BlendTime { get; set; }
	}
}