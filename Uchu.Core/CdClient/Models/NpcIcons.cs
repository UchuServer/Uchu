using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Uchu.Core.CdClient
{
	[Table("NpcIcons")]
	public class NpcIcons
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[Column("id")]
		public int? Id { get; set; }

		[Column("color")]
		public int? Color { get; set; }

		[Column("offset")]
		public float? Offset { get; set; }

		[Column("LOT")]
		public int? LOT { get; set; }

		[Column("Texture")]
		public string Texture { get; set; }

		[Column("isClickable")]
		public bool? IsClickable { get; set; }

		[Column("scale")]
		public float? Scale { get; set; }

		[Column("rotateToFace")]
		public bool? RotateToFace { get; set; }

		[Column("compositeHorizOffset")]
		public float? CompositeHorizOffset { get; set; }

		[Column("compositeVertOffset")]
		public float? CompositeVertOffset { get; set; }

		[Column("compositeScale")]
		public float? CompositeScale { get; set; }

		[Column("compositeConnectionNode")]
		public string CompositeConnectionNode { get; set; }

		[Column("compositeLOTMultiMission")]
		public int? CompositeLOTMultiMission { get; set; }

		[Column("compositeLOTMultiMissionVentor")]
		public int? CompositeLOTMultiMissionVentor { get; set; }

		[Column("compositeIconTexture")]
		public string CompositeIconTexture { get; set; }
	}
}