using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Uchu.Core.CdClient
{
	[Table("ZoneSummary")]
	public class ZoneSummary
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[Column("zoneID")]
		public int? ZoneID { get; set; }

		[Column("type")]
		public int? Type { get; set; }

		[Column("value")]
		public int? Value { get; set; }

		[Column("_uniqueID")]
		public int? UniqueID { get; set; }
	}
}