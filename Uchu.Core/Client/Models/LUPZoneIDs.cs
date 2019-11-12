using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Uchu.Core.Client
{
	[Table("LUPZoneIDs")]
	public class LUPZoneIDs
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[Column("zoneID")]
		public int? ZoneID { get; set; }
	}
}