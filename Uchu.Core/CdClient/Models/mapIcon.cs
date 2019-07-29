using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Uchu.Core.CdClient
{
	[Table("mapIcon")]
	public class mapIcon
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[Column("LOT")]
		public int? LOT { get; set; }

		[Column("iconID")]
		public int? IconID { get; set; }

		[Column("iconState")]
		public int? IconState { get; set; }
	}
}