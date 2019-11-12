using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Uchu.Core.Client
{
	[Table("PlayerFlags")]
	public class PlayerFlags
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[Column("id")]
		public int? Id { get; set; }

		[Column("SessionOnly")]
		public bool? SessionOnly { get; set; }

		[Column("OnlySetByServer")]
		public bool? OnlySetByServer { get; set; }

		[Column("SessionZoneOnly")]
		public bool? SessionZoneOnly { get; set; }
	}
}