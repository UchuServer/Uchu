using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Uchu.Core.Client
{
	[Table("TrailEffects")]
	public class TrailEffects
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[Column("trailID")]
		public int? TrailID { get; set; }

		[Column("textureName")]
		public string TextureName { get; set; }

		[Column("blendmode")]
		public int? Blendmode { get; set; }

		[Column("cardlifetime")]
		public float? Cardlifetime { get; set; }

		[Column("colorlifetime")]
		public float? Colorlifetime { get; set; }

		[Column("minTailFade")]
		public float? MinTailFade { get; set; }

		[Column("tailFade")]
		public float? TailFade { get; set; }

		[Column("max_particles")]
		public int? Maxparticles { get; set; }

		[Column("birthDelay")]
		public float? BirthDelay { get; set; }

		[Column("deathDelay")]
		public float? DeathDelay { get; set; }

		[Column("bone1")]
		public string Bone1 { get; set; }

		[Column("bone2")]
		public string Bone2 { get; set; }

		[Column("texLength")]
		public float? TexLength { get; set; }

		[Column("texWidth")]
		public float? TexWidth { get; set; }

		[Column("startColorR")]
		public float? StartColorR { get; set; }

		[Column("startColorG")]
		public float? StartColorG { get; set; }

		[Column("startColorB")]
		public float? StartColorB { get; set; }

		[Column("startColorA")]
		public float? StartColorA { get; set; }

		[Column("middleColorR")]
		public float? MiddleColorR { get; set; }

		[Column("middleColorG")]
		public float? MiddleColorG { get; set; }

		[Column("middleColorB")]
		public float? MiddleColorB { get; set; }

		[Column("middleColorA")]
		public float? MiddleColorA { get; set; }

		[Column("endColorR")]
		public float? EndColorR { get; set; }

		[Column("endColorG")]
		public float? EndColorG { get; set; }

		[Column("endColorB")]
		public float? EndColorB { get; set; }

		[Column("endColorA")]
		public float? EndColorA { get; set; }
	}
}