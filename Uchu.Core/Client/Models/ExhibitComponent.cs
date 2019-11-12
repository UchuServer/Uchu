using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Uchu.Core.Client
{
	[Table("ExhibitComponent")]
	public class ExhibitComponent
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[Column("id")]
		public int? Id { get; set; }

		[Column("length")]
		public float? Length { get; set; }

		[Column("width")]
		public float? Width { get; set; }

		[Column("height")]
		public float? Height { get; set; }

		[Column("offsetX")]
		public float? OffsetX { get; set; }

		[Column("offsetY")]
		public float? OffsetY { get; set; }

		[Column("offsetZ")]
		public float? OffsetZ { get; set; }

		[Column("fReputationSizeMultiplier")]
		public float? FReputationSizeMultiplier { get; set; }

		[Column("fImaginationCost")]
		public float? FImaginationCost { get; set; }
	}
}