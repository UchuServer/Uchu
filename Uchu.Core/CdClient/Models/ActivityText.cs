using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Uchu.Core.CdClient
{
	[Table("ActivityText")]
	public class ActivityText
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[Column("activityID")]
		public int? ActivityID { get; set; }

		[Column("type")]
		public string Type { get; set; }

		[Column("localize")]
		public bool? Localize { get; set; }

		[Column("locStatus")]
		public int? LocStatus { get; set; }

		[Column("gate_version")]
		public string Gateversion { get; set; }
	}
}