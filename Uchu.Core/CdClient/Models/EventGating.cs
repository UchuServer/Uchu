using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Uchu.Core.CdClient
{
	[Table("EventGating")]
	public class EventGating
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[Column("eventName")]
		public string EventName { get; set; }

		[Column("date_start")]
		public long? Datestart { get; set; }

		[Column("date_end")]
		public long? Dateend { get; set; }
	}
}