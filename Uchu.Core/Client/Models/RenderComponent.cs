using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Uchu.Core.Client
{
	[Table("RenderComponent")]
	public class RenderComponent
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[Column("id")]
		public int? Id { get; set; }

		[Column("render_asset")]
		public string Renderasset { get; set; }

		[Column("icon_asset")]
		public string Iconasset { get; set; }

		[Column("IconID")]
		public int? IconID { get; set; }

		[Column("shader_id")]
		public int? Shaderid { get; set; }

		[Column("effect1")]
		public int? Effect1 { get; set; }

		[Column("effect2")]
		public int? Effect2 { get; set; }

		[Column("effect3")]
		public int? Effect3 { get; set; }

		[Column("effect4")]
		public int? Effect4 { get; set; }

		[Column("effect5")]
		public int? Effect5 { get; set; }

		[Column("effect6")]
		public int? Effect6 { get; set; }

		[Column("animationGroupIDs")]
		public string AnimationGroupIDs { get; set; }

		[Column("fade")]
		public bool? Fade { get; set; }

		[Column("usedropshadow")]
		public bool? Usedropshadow { get; set; }

		[Column("preloadAnimations")]
		public bool? PreloadAnimations { get; set; }

		[Column("fadeInTime")]
		public float? FadeInTime { get; set; }

		[Column("maxShadowDistance")]
		public float? MaxShadowDistance { get; set; }

		[Column("ignoreCameraCollision")]
		public bool? IgnoreCameraCollision { get; set; }

		[Column("renderComponentLOD1")]
		public int? RenderComponentLOD1 { get; set; }

		[Column("renderComponentLOD2")]
		public int? RenderComponentLOD2 { get; set; }

		[Column("gradualSnap")]
		public bool? GradualSnap { get; set; }

		[Column("animationFlag")]
		public int? AnimationFlag { get; set; }

		[Column("AudioMetaEventSet")]
		public string AudioMetaEventSet { get; set; }

		[Column("billboardHeight")]
		public float? BillboardHeight { get; set; }

		[Column("chatBubbleOffset")]
		public float? ChatBubbleOffset { get; set; }

		[Column("staticBillboard")]
		public bool? StaticBillboard { get; set; }

		[Column("LXFMLFolder")]
		public string LXFMLFolder { get; set; }

		[Column("attachIndicatorsToNode")]
		public bool? AttachIndicatorsToNode { get; set; }
	}
}