using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Uchu.Core.CdClient
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