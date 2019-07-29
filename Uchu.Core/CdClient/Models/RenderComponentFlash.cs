using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Uchu.Core.CdClient
{
	[Table("RenderComponentFlash")]
	public class RenderComponentFlash
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[Column("id")]
		public int? Id { get; set; }

		[Column("interactive")]
		public bool? Interactive { get; set; }

		[Column("animated")]
		public bool? Animated { get; set; }

		[Column("nodeName")]
		public string NodeName { get; set; }

		[Column("flashPath")]
		public string FlashPath { get; set; }

		[Column("elementName")]
		public string ElementName { get; set; }

		[Column("_uid")]
		public int? Uid { get; set; }
	}
}