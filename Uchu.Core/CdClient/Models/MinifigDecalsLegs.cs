using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Uchu.Core.CdClient
{
	[Table("MinifigDecals_Legs")]
	public class MinifigDecalsLegs
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[Column("ID")]
		public int? ID { get; set; }

		[Column("High_path")]
		public string Highpath { get; set; }
	}
}