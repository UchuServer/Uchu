using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Uchu.Core.Client
{
	[Table("PlayerStatistics")]
	public class PlayerStatistics
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[Column("statID")]
		public int? StatID { get; set; }

		[Column("sortOrder")]
		public int? SortOrder { get; set; }

		[Column("locStatus")]
		public int? LocStatus { get; set; }

		[Column("gate_version")]
		public string Gateversion { get; set; }
	}
}