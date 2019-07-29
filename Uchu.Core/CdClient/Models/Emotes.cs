using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Uchu.Core.CdClient
{
	[Table("Emotes")]
	public class Emotes
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[Column("id")]
		public int? Id { get; set; }

		[Column("animationName")]
		public string AnimationName { get; set; }

		[Column("iconFilename")]
		public string IconFilename { get; set; }

		[Column("channel")]
		public string Channel { get; set; }

		[Column("command")]
		public string Command { get; set; }

		[Column("locked")]
		public bool? Locked { get; set; }

		[Column("localize")]
		public bool? Localize { get; set; }

		[Column("locStatus")]
		public int? LocStatus { get; set; }

		[Column("gate_version")]
		public string Gateversion { get; set; }
	}
}