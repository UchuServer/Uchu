using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Uchu.Core.CdClient
{
	[Table("BrickIDTable")]
	public class BrickIDTable
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[Column("NDObjectID")]
		public int? NDObjectID { get; set; }

		[Column("LEGOBrickID")]
		public int? LEGOBrickID { get; set; }
	}
}