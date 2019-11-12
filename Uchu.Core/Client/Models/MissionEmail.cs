using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Uchu.Core.Client
{
	[Table("MissionEmail")]
	public class MissionEmail
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[Column("ID")]
		public int? ID { get; set; }

		[Column("messageType")]
		public int? MessageType { get; set; }

		[Column("notificationGroup")]
		public int? NotificationGroup { get; set; }

		[Column("missionID")]
		public int? MissionID { get; set; }

		[Column("attachmentLOT")]
		public int? AttachmentLOT { get; set; }

		[Column("localize")]
		public bool? Localize { get; set; }

		[Column("locStatus")]
		public int? LocStatus { get; set; }

		[Column("gate_version")]
		public string Gateversion { get; set; }
	}
}