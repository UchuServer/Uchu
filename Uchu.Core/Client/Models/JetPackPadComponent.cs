using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Uchu.Core.Client
{
	[Table("JetPackPadComponent")]
	public class JetPackPadComponent
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[Column("id")]
		public int? Id { get; set; }

		[Column("xDistance")]
		public float? XDistance { get; set; }

		[Column("yDistance")]
		public float? YDistance { get; set; }

		[Column("warnDistance")]
		public float? WarnDistance { get; set; }

		[Column("lotBlocker")]
		public int? LotBlocker { get; set; }

		[Column("lotWarningVolume")]
		public int? LotWarningVolume { get; set; }
	}
}