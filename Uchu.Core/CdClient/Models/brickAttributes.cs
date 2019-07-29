using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Uchu.Core.CdClient
{
	[Table("brickAttributes")]
	public class brickAttributes
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[Column("ID")]
		public int? ID { get; set; }

		[Column("icon_asset")]
		public string Iconasset { get; set; }

		[Column("display_order")]
		public int? Displayorder { get; set; }

		[Column("locStatus")]
		public int? LocStatus { get; set; }
	}
}