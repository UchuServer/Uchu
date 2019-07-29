using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Uchu.Core.CdClient
{
	[Table("SpeedchatMenu")]
	public class SpeedchatMenu
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[Column("id")]
		public int? Id { get; set; }

		[Column("parentId")]
		public int? ParentId { get; set; }

		[Column("emoteId")]
		public int? EmoteId { get; set; }

		[Column("imageName")]
		public string ImageName { get; set; }

		[Column("localize")]
		public bool? Localize { get; set; }

		[Column("locStatus")]
		public int? LocStatus { get; set; }

		[Column("gate_version")]
		public string Gateversion { get; set; }
	}
}